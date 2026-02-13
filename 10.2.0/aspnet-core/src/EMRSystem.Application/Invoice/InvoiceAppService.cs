using Abp.Application.Services;
using Abp.Domain.Repositories;
using Abp.EntityFrameworkCore.Repositories;
using Abp.Extensions;
using Abp.UI;
using EFCore.BulkExtensions;
using EMRSystem.Appointments;
using EMRSystem.Deposit;
using EMRSystem.EmergencyChargeEntries;
using EMRSystem.Insurances;
using EMRSystem.Invoice.Dto;
using EMRSystem.Invoices;
using EMRSystem.IpdChargeEntry;
using EMRSystem.IpdChargeEntry.Dto;
using EMRSystem.NumberingService;
using EMRSystem.Patients;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Stripe;
using Stripe.Checkout;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using InvoiceItem = EMRSystem.Invoices.InvoiceItem;
using PaymentMethod = EMRSystem.Invoices.PaymentMethod;

namespace EMRSystem.Invoice
{
    public class InvoiceAppService : AsyncCrudAppService<EMRSystem.Invoices.Invoice, InvoiceDto, long,
     PagedInvoiceResultRequestDto, CreateUpdateInvoiceDto, CreateUpdateInvoiceDto>,
     IInvoiceAppService
    {
        private readonly IConfiguration _configuration;
        private readonly INumberingService _numberingService;
        private readonly IRepository<EMRSystem.Admission.Admission, long> _admissionRepository;
        private readonly IRepository<InsuranceClaim, long> _insuranceClaimRepository;
        private readonly IRepository<PatientInsurance, long> _patientInsuranceRepository;
        private readonly IRepository<PatientDeposit, long> _patientDepositRepository;
        private readonly IRepository<Patient, long> _patientRepository;
        private readonly IRepository<DepositTransaction, long> _depositTransactionRepository;
        private readonly IRepository<EMRSystem.IpdChargeEntry.IpdChargeEntry, long> _ipdChargeEntryRepository;
        private readonly IRepository<EMRSystem.EmergencyChargeEntries.EmergencyChargeEntry, long> _emergencyChargeEntriesRepository;
        public InvoiceAppService(
            IRepository<EMRSystem.Invoices.Invoice, long> repository,
            INumberingService numberingService,
            IRepository<Patient, long> patientRepository,
            IRepository<EMRSystem.Admission.Admission, long> admissionRepository,
            IRepository<InsuranceClaim, long> insuranceClaimRepository,
            IRepository<PatientInsurance, long> patientInsuranceRepository,
            IRepository<PatientDeposit, long> patientDepositRepository,
            IRepository<DepositTransaction, long> depositTransactionRepository,
            IRepository<EMRSystem.EmergencyChargeEntries.EmergencyChargeEntry, long> emergencyChargeEntriesRepository,
            IConfiguration configuration,
            IRepository<EMRSystem.IpdChargeEntry.IpdChargeEntry, long> ipdChargeEntryRepository) : base(repository)
        {
            _configuration = configuration;
            _numberingService = numberingService;
            _admissionRepository = admissionRepository;
            _insuranceClaimRepository = insuranceClaimRepository;
            _patientInsuranceRepository = patientInsuranceRepository;
            _patientRepository = patientRepository;
            _patientDepositRepository = patientDepositRepository;
            _depositTransactionRepository = depositTransactionRepository;
            _ipdChargeEntryRepository = ipdChargeEntryRepository;
            _emergencyChargeEntriesRepository = emergencyChargeEntriesRepository;
            StripeConfiguration.ApiKey = _configuration["Stripe:SecretKey"];
        }

        protected override IQueryable<EMRSystem.Invoices.Invoice> CreateFilteredQuery(PagedInvoiceResultRequestDto input)
        {
            // Use IQueryable type to avoid conversion issues
            IQueryable<EMRSystem.Invoices.Invoice> query = Repository.GetAll()
                .Include(x => x.Patient)
                .Include(x => x.InsuranceClaims).ThenInclude(pi => pi.PatientInsurance).ThenInclude(i => i.InsuranceMaster); // EF Core handles multiple Includes

            if (!input.Keyword.IsNullOrWhiteSpace())
            {
                query = query.Where(x =>
                    (x.Patient.FullName != null && x.Patient.FullName.Contains(input.Keyword)) ||
                    (x.InvoiceNo != null && x.InvoiceNo.Contains(input.Keyword)));
            }

            return query;
        }


        protected override IQueryable<EMRSystem.Invoices.Invoice> ApplySorting(IQueryable<EMRSystem.Invoices.Invoice> query, PagedInvoiceResultRequestDto input)
        {
            if (!string.IsNullOrWhiteSpace(input.Sorting))
            {
                var sorting = input.Sorting;

                if (sorting.Contains("patientName", StringComparison.OrdinalIgnoreCase))
                    sorting = sorting.Replace("patientName", "Patient.FullName", StringComparison.OrdinalIgnoreCase);

                if (sorting.Contains("invoiceDate", StringComparison.OrdinalIgnoreCase))
                    sorting = sorting.Replace("invoiceDate", "InvoiceDate", StringComparison.OrdinalIgnoreCase);

                if (sorting.Contains("totalAmount", StringComparison.OrdinalIgnoreCase))
                    sorting = sorting.Replace("totalAmount", "TotalAmount", StringComparison.OrdinalIgnoreCase);

                return query.OrderBy(sorting);
            }

            return query.OrderByDescending(x => x.InvoiceDate);
        }

        private async Task<string> GenerateInvoiceNoAsync(int tenantId)
        {
            return await _numberingService.GenerateReceiptNumberAsync(
                Repository,         // Invoice repository
                "INV",              // prefix
                tenantId,
                "InvoiceNo"         // property name from Invoice entity
            );
        }


        public override async Task<InvoiceDto> CreateAsync(CreateUpdateInvoiceDto input)
        {
            using (var uow = UnitOfWorkManager.Begin())
            {
                var dbContext = Repository.GetDbContext();

                // Step 1: Prepare invoice
                var invoice = ObjectMapper.Map<Invoices.Invoice>(input);
                invoice.InvoiceNo = await GenerateInvoiceNoAsync(input.TenantId);

                var activeAdmission = await _admissionRepository.FirstOrDefaultAsync(x =>
                    x.PatientId == input.PatientId && !x.IsDischarged);

                var billingMode = activeAdmission?.BillingMode;
                invoice.Status = (billingMode == null || billingMode == BillingMethod.SelfPay)
                    ? InvoiceStatus.Paid
                    : InvoiceStatus.Unpaid;

                await dbContext.Set<Invoices.Invoice>().AddAsync(invoice);
                await dbContext.SaveChangesAsync(); // To get Invoice.Id

                // Step 2: Bulk insert invoice items
                if (input.Items != null && input.Items.Any())
                {
                    var invoiceItems = input.Items.Select(item =>
                    {
                        var entity = ObjectMapper.Map<InvoiceItem>(item);
                        entity.InvoiceId = invoice.Id;
                        return entity;
                    }).ToList();

                    await dbContext.BulkInsertAsync(invoiceItems, new BulkConfig
                    {
                        PreserveInsertOrder = true,
                        SetOutputIdentity = false
                    });
                }

                // Step 3: Billing Mode Handling
                if (billingMode == null || billingMode == BillingMethod.SelfPay)
                {
                    var patientDeposit = await _patientDepositRepository.FirstOrDefaultAsync(x =>
                        x.PatientId == input.PatientId && x.TenantId == input.TenantId);

                    if (patientDeposit == null)
                        throw new UserFriendlyException("Patient Deposit record not found.");

                    decimal remainingToDebit = input.TotalAmount;

                    // FIFO credit selection
                    var creditEntries = await _depositTransactionRepository.GetAll()
                        .Where(x => x.PatientDepositId == patientDeposit.Id
                            && x.TransactionType == TransactionType.Credit
                            && x.RemainingAmount > 0)
                        .OrderBy(x => x.TransactionDate)
                        .ToListAsync();

                    foreach (var credit in creditEntries)
                    {
                        if (remainingToDebit <= 0)
                            break;

                        var deduct = Math.Min(credit.RemainingAmount, remainingToDebit);
                        credit.RemainingAmount -= deduct;
                        remainingToDebit -= deduct;
                    }

                    if (creditEntries.Any())
                        await dbContext.BulkUpdateAsync(creditEntries, new BulkConfig { PreserveInsertOrder = true });

                    // Debit entry
                    var debitTransaction = new DepositTransaction
                    {
                        TenantId = input.TenantId,
                        PatientDepositId = patientDeposit.Id,
                        Amount = input.TotalAmount,
                        TransactionType = TransactionType.Debit,
                        TransactionDate = DateTime.Now,
                        Description = $"Invoice {invoice.InvoiceNo}",
                        IsPaid = false,
                        RemainingAmount = 0
                    };

                    await dbContext.Set<DepositTransaction>().AddAsync(debitTransaction);

                    patientDeposit.TotalDebitAmount += input.TotalAmount;
                    patientDeposit.TotalBalance = patientDeposit.TotalCreditAmount - patientDeposit.TotalDebitAmount;
                    dbContext.Set<PatientDeposit>().Update(patientDeposit); // ✅ sync Update
                }
                else
                {
                    var patientInsurance = await _patientInsuranceRepository.FirstOrDefaultAsync(x =>
                        x.PatientId == input.PatientId && x.IsActive);

                    if (patientInsurance == null)
                        throw new UserFriendlyException("No active insurance found for this patient.");

                    var claim = new InsuranceClaim
                    {
                        TenantId = input.TenantId,
                        InvoiceId = invoice.Id,
                        PatientInsuranceId = patientInsurance.Id,
                        TotalAmount = input.TotalAmount,
                        Status = ClaimStatus.Pending,
                        CreatedDate = DateTime.Now
                    };

                    await dbContext.Set<InsuranceClaim>().AddAsync(claim);
                    await dbContext.SaveChangesAsync();

                    invoice.InsuranceClaimId = claim.Id;
                    invoice.IsClaimGenerated = true;
                    dbContext.Set<Invoices.Invoice>().Update(invoice); // ✅ sync Update
                }

                // Step 4: Handle emergency/IPD charges in bulk
                var patient = await _patientRepository.FirstOrDefaultAsync(x => x.Id == input.PatientId);
                if (patient != null && patient.IsEmergencyCharge)
                {
                    patient.IsEmergencyCharge = false;
                    dbContext.Set<Patient>().Update(patient); // ✅ sync Update
                }

                var emergencyCharges = await _emergencyChargeEntriesRepository.GetAll()
                    .Where(x => x.PatientId == input.PatientId && !x.IsProcessed)
                    .ToListAsync();

                if (emergencyCharges.Any())
                {
                    emergencyCharges.ForEach(c => c.IsProcessed = true);
                    await dbContext.BulkUpdateAsync(emergencyCharges);
                }

                var charges = await GetChargesByPatientAsync(input.PatientId, input.InvoiceType);
                if (charges.Any())
                {
                    var chargeIds = charges.Select(c => c.Id).ToList();
                    var chargeEntities = await _ipdChargeEntryRepository.GetAll()
                        .Where(x => chargeIds.Contains(x.Id))
                        .ToListAsync();

                    chargeEntities.ForEach(c => c.IsProcessed = true);
                    await dbContext.BulkUpdateAsync(chargeEntities);
                }

                await dbContext.SaveChangesAsync();
                await uow.CompleteAsync();

                return MapToEntityDto(invoice);
            }
        }

        public virtual async Task<InvoiceDto> CollectCoPayAsync(long invoiceId)
        {
            // Step 1: Fetch invoice
            var invoice = await Repository.GetAllIncluding(x => x.Items)
                .FirstOrDefaultAsync(x => x.Id == invoiceId);

            if (invoice == null)
                throw new UserFriendlyException("Invoice not found.");

            // Step 2: Get Patient Deposit
            var patientDeposit = await _patientDepositRepository.FirstOrDefaultAsync(x =>
                x.PatientId == invoice.PatientId && x.TenantId == invoice.TenantId);

            if (patientDeposit == null)
                throw new UserFriendlyException("Patient deposit record not found.");

            // Step 3: Validate CoPay
            var coPayAmount = invoice.CoPayAmount ?? 0;
            if (coPayAmount <= 0)
                throw new UserFriendlyException("No CoPay amount to collect.");

            decimal remainingToDebit = coPayAmount;

            // ✅ FIFO — pick oldest credit entries with remaining balance
            var creditEntries = await _depositTransactionRepository.GetAll()
                .Where(x => x.PatientDepositId == patientDeposit.Id
                    && x.TransactionType == TransactionType.Credit
                    && x.RemainingAmount > 0)
                .OrderBy(x => x.TransactionDate)
                .ToListAsync();

            foreach (var credit in creditEntries)
            {
                if (remainingToDebit <= 0) break;

                var deduct = Math.Min(credit.RemainingAmount, remainingToDebit);

                credit.RemainingAmount -= deduct;
                remainingToDebit -= deduct;

                await _depositTransactionRepository.UpdateAsync(credit);
            }

            // Step 4: Insert Debit Transaction entry
            var transaction = new DepositTransaction
            {
                TenantId = invoice.TenantId,
                PatientDepositId = patientDeposit.Id,
                Amount = coPayAmount,
                TransactionType = TransactionType.Debit,
                PaymentMethod = null, // cash/card logic separate
                TransactionDate = DateTime.Now,
                Description = $"CoPay collected for Invoice {invoice.InvoiceNo}",
                ReceiptNo = null,
                IsPaid = false,

                // FIFO tracking fields
                RemainingAmount = 0,
                RefundedAmount = 0,
                IsRefund = false,
                RefundTransactionId = null,
                RefundDate = null,
                RefundReceiptNo = null
            };

            await _depositTransactionRepository.InsertAsync(transaction);

            // Step 5: Update main deposit account totals
            patientDeposit.TotalDebitAmount += coPayAmount;
            patientDeposit.TotalBalance = patientDeposit.TotalCreditAmount - patientDeposit.TotalDebitAmount;
            await _patientDepositRepository.UpdateAsync(patientDeposit);

            // Step 6: Update Invoice Status
            invoice.Status = InvoiceStatus.CollectedCoPayAmount;
            await Repository.UpdateAsync(invoice);

            await CurrentUnitOfWork.SaveChangesAsync();

            return MapToEntityDto(invoice);
        }


        public async Task<List<IpdChargeEntryDto>> GetChargesByPatientAsync(long patientId, InvoiceType invoiceType)
        {
            // 🔹 Patient fetch kar le
            var patient = await _patientRepository.GetAsync(patientId);

            //var today = DateTime.Today;

            // 🔹 IPD Charges
            var ipdQuery = _ipdChargeEntryRepository
                .GetAllIncluding(x => x.Patient, x => x.Admission)
                .Where(x => x.PatientId == patientId && !x.IsProcessed);

            //if (invoiceType == InvoiceType.DailyInvoice)
            //{
            //    ipdQuery = ipdQuery.Where(x => x.EntryDate.Date == today);
            //}

            var ipdCharges = await ipdQuery.ToListAsync();

            // 🔹 Emergency Charges (agar patient emergency ka hai)
            var emergencyCharges = new List<EmergencyChargeEntry>();
            if (patient.IsEmergencyCharge)
            {
                // ⚡ Sirf IsProcessed check karna hai, EntryDate filter nahi lagega
                emergencyCharges = await _emergencyChargeEntriesRepository
                    .GetAllIncluding(x => x.Patient, x => x.EmergencyCase)
                    .Where(x => x.PatientId == patientId && !x.IsProcessed)
                    .ToListAsync();
            }

            // 🔹 Map to DTO
            var ipdDtos = ObjectMapper.Map<List<IpdChargeEntryDto>>(ipdCharges);

            // ⚡ EmergencyChargeEntry ko bhi IpdChargeEntryDto me map karna padega
            var emergencyDtos = emergencyCharges.Select(e => new IpdChargeEntryDto
            {
                Id = e.Id,
                PatientId = e.PatientId ?? 0, // null safe
                ChargeType = e.ChargeType.ToString(),
                Description = e.Description,
                Amount = e.Amount,
                Quantity = e.Quantity,
                EntryDate = e.EntryDate,
                IsProcessed = e.IsProcessed,
                ReferenceId = e.ReferenceId
            }).ToList();

            // 🔹 Combine both
            var allCharges = ipdDtos.Concat(emergencyDtos).ToList();

            return allCharges;
        }

        public async Task<InvoiceDto> GetInvoiceWithItemsAsync(long id)
        {
            var invoice = await Repository
                .GetAll()
                .Include(x => x.Items)
                .Include(x => x.Patient)
                .Include(x => x.InsuranceClaims)
                    .ThenInclude(pi => pi.PatientInsurance)
                        .ThenInclude(i => i.InsuranceMaster)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (invoice == null)
                throw new Abp.UI.UserFriendlyException("Invoice not found");

            // Map base invoice first
            var invoiceDto = ObjectMapper.Map<InvoiceDto>(invoice);

            // ✅ If this invoice is Final Invoice, include all linked invoices' items
            if (invoice.IsFinalInvoice)
            {
                var childInvoices = await Repository
                    .GetAll()
                    .Include(x => x.Items)
                    .Where(x => x.FinalInvoiceId == id)
                    .ToListAsync();

                // merge all items from child invoices
                var allItems = new List<InvoiceItem>();

                // include base invoice items
                allItems.AddRange(invoice.Items);

                // include child invoice items
                foreach (var child in childInvoices)
                {
                    allItems.AddRange(child.Items);
                }

                // map merged items to DTO
                invoiceDto.Items = ObjectMapper.Map<List<InvoiceItemDto>>(allItems);
            }

            return invoiceDto;
        }

        public async Task<List<InvoiceDto>> GetInvoicesByPatientID(long patientID)
        {
            var list = await Repository.GetAll().Where(x => x.PatientId == patientID).ToListAsync();
            var mappedList = ObjectMapper.Map<List<InvoiceDto>>(list);
            return mappedList;
        }
        public async Task<long> GenerateFinalInvoiceAsync(long patientId)
        {
            // 1️⃣ Get all unpaid/unconverted daily invoices of this patient
            var dailyInvoices = await Repository.GetAll()
                .Where(x => x.PatientId == patientId
                         && x.InvoiceType == InvoiceType.DailyInvoice
                         && !x.IsConvertedToFinalInvoice)
                .ToListAsync();

            if (!dailyInvoices.Any())
                throw new UserFriendlyException("No pending daily invoices found to convert.");

            // 2️⃣ Calculate total / approved / copay
            decimal totalAmount = dailyInvoices.Sum(x => x.TotalAmount);
            decimal approvedAmount = dailyInvoices.Sum(x => x.ApprovedAmount ?? 0);
            decimal coPayAmount = dailyInvoices.Sum(x => x.CoPayAmount ?? 0);

            // 3️⃣ Create new Final Invoice
            var finalInvoice = new EMRSystem.Invoices.Invoice
            {
                TenantId = dailyInvoices.First().TenantId,
                PatientId = patientId,
                InvoiceNo = await GenerateInvoiceNoAsync(dailyInvoices.First().TenantId), // reuse your existing number generator
                InvoiceType = InvoiceType.FullInvoice,
                InvoiceDate = DateTime.Now,
                TotalAmount = totalAmount,
                ApprovedAmount = approvedAmount,
                CoPayAmount = coPayAmount,
                IsFinalInvoice = true,
                Status = InvoiceStatus.Paid
            };

            var finalInvoiceId = await Repository.InsertAndGetIdAsync(finalInvoice);
            await CurrentUnitOfWork.SaveChangesAsync();

            // 4️⃣ Update old daily invoices
            foreach (var inv in dailyInvoices)
            {
                inv.IsConvertedToFinalInvoice = true;
                inv.FinalInvoiceId = finalInvoiceId;
                await Repository.UpdateAsync(inv);
            }

            await CurrentUnitOfWork.SaveChangesAsync();

            return finalInvoiceId;
        }

    }
}
