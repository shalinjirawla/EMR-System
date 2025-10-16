using Abp.Application.Services;
using Abp.Domain.Repositories;
using Abp.EntityFrameworkCore.Repositories;
using Abp.Extensions;
using Abp.UI;
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
            // Step 1: Map invoice from input
            var invoice = ObjectMapper.Map<Invoices.Invoice>(input);
            invoice.InvoiceNo = await GenerateInvoiceNoAsync(input.TenantId);

            // Step 2: Check patient admission & billing mode
            var activeAdmission = await _admissionRepository.FirstOrDefaultAsync(x =>
                x.PatientId == input.PatientId &&
                !x.IsDischarged);

            BillingMethod? billingMode = activeAdmission?.BillingMode;

            // Step 3: Save invoice basic data
            invoice.Status = (billingMode == null || billingMode == BillingMethod.SelfPay)
                ? InvoiceStatus.Paid
                : InvoiceStatus.Unpaid;

            await Repository.InsertAsync(invoice);
            await CurrentUnitOfWork.SaveChangesAsync();

            // Step 4: Save invoice items
            foreach (var item in input.Items)
            {
                var invoiceItem = ObjectMapper.Map<InvoiceItem>(item);
                invoiceItem.InvoiceId = invoice.Id;
                await Repository.GetDbContext().Set<InvoiceItem>().AddAsync(invoiceItem);
            }
            await CurrentUnitOfWork.SaveChangesAsync();

            // Step 5: If billing is SELF PAY → handle deposit logic
            if (billingMode == null || billingMode == BillingMethod.SelfPay)
            {
                var patientDeposit = await _patientDepositRepository.FirstOrDefaultAsync(x =>
                    x.PatientId == input.PatientId && x.TenantId == input.TenantId);

                if (patientDeposit == null)
                {
                    throw new UserFriendlyException("Patient Deposit record not found.");
                }

                var transaction = new DepositTransaction
                {
                    TenantId = input.TenantId,
                    PatientDepositId = patientDeposit.Id,
                    Amount = input.TotalAmount,
                    TransactionType = TransactionType.Debit,
                    PaymentMethod = null,
                    TransactionDate = DateTime.Now,
                    Description = $"Invoice {invoice.InvoiceNo}",
                    ReceiptNo = null,
                    IsPaid = false
                };
                await _depositTransactionRepository.InsertAsync(transaction);

                patientDeposit.TotalDebitAmount += input.TotalAmount;
                patientDeposit.TotalBalance = patientDeposit.TotalCreditAmount - patientDeposit.TotalDebitAmount;

                await _patientDepositRepository.UpdateAsync(patientDeposit);
                await CurrentUnitOfWork.SaveChangesAsync();
            }
            else
            {
                // Step 6: Billing is Insurance — create claim entry

                // Get PatientInsuranceId
                var patientInsurance = await _patientInsuranceRepository.FirstOrDefaultAsync(x =>
                    x.PatientId == input.PatientId && x.IsActive);

                if (patientInsurance == null)
                {
                    throw new UserFriendlyException("No active insurance found for this patient.");
                }

                // Create InsuranceClaim
                var claim = new InsuranceClaim
                {
                    TenantId = input.TenantId,
                    InvoiceId = invoice.Id,
                    PatientInsuranceId = patientInsurance.Id,
                    TotalAmount = input.TotalAmount,
                    AmountPayByInsurance = 0,
                    AmountPayByPatient = 0,
                    Status = ClaimStatus.Pending,
                    Remarks = null,
                    CreatedDate = DateTime.Now
                };

                await _insuranceClaimRepository.InsertAsync(claim);
                await CurrentUnitOfWork.SaveChangesAsync();

                // Link claim to invoice
                invoice.InsuranceClaimId = claim.Id;
                invoice.IsClaimGenerated = true;
                await Repository.UpdateAsync(invoice);
                await CurrentUnitOfWork.SaveChangesAsync();
            }

            // Step 7: Process emergency and ipd charges (same as before)
            var patient = await _patientRepository.FirstOrDefaultAsync(x => x.Id == input.PatientId);
            if (patient != null && patient.IsEmergencyCharge)
            {
                patient.IsEmergencyCharge = false;
                await _patientRepository.UpdateAsync(patient);
            }

            var emergencyCharges = await _emergencyChargeEntriesRepository.GetAllListAsync(x =>
                x.PatientId == input.PatientId && !x.IsProcessed);

            if (emergencyCharges.Any())
            {
                foreach (var charge in emergencyCharges)
                {
                    charge.IsProcessed = true;
                    await _emergencyChargeEntriesRepository.UpdateAsync(charge);
                }
            }

            var charges = await GetChargesByPatientAsync(input.PatientId, input.InvoiceType);
            if (charges.Any())
            {
                foreach (var chargeDto in charges)
                {
                    var chargeEntity = await _ipdChargeEntryRepository.GetAsync(chargeDto.Id);
                    chargeEntity.IsProcessed = true;
                    await _ipdChargeEntryRepository.UpdateAsync(chargeEntity);
                }
            }

            await CurrentUnitOfWork.SaveChangesAsync();

            // Step 8: Return DTO
            return MapToEntityDto(invoice);
        }

        public virtual async Task<InvoiceDto> CollectCoPayAsync(long invoiceId)
        {
            // Step 1: Fetch the invoice with related items
            var invoice = await Repository.GetAllIncluding(x => x.Items)
                .FirstOrDefaultAsync(x => x.Id == invoiceId);

            if (invoice == null)
                throw new UserFriendlyException("Invoice not found.");

            // Step 2: Fetch patient deposit record
            var patientDeposit = await _patientDepositRepository.FirstOrDefaultAsync(x =>
                x.PatientId == invoice.PatientId && x.TenantId == invoice.TenantId);

            if (patientDeposit == null)
                throw new UserFriendlyException("Patient deposit record not found.");

            // Step 3: Create a deposit transaction for CoPay
            var coPayAmount = invoice.CoPayAmount ?? 0;
            if (coPayAmount <= 0)
                throw new UserFriendlyException("No CoPay amount to collect.");

            var transaction = new DepositTransaction
            {
                TenantId = invoice.TenantId,
                PatientDepositId = patientDeposit.Id,
                Amount = coPayAmount,
                TransactionType = TransactionType.Debit,
                PaymentMethod = null, // can be Cash/Card
                TransactionDate = DateTime.Now,
                Description = $"CoPay collected for Invoice {invoice.InvoiceNo}",
                ReceiptNo = null,
                IsPaid = false
            };

            await _depositTransactionRepository.InsertAsync(transaction);

            // Step 4: Update patient deposit balances
            patientDeposit.TotalDebitAmount += coPayAmount;
            patientDeposit.TotalBalance = patientDeposit.TotalCreditAmount - patientDeposit.TotalDebitAmount;
            await _patientDepositRepository.UpdateAsync(patientDeposit);

            // Step 5: Update invoice status
            invoice.Status = InvoiceStatus.CollectedCoPayAmount;
            await Repository.UpdateAsync(invoice);

            await CurrentUnitOfWork.SaveChangesAsync();

            // Step 6: Return updated invoice DTO
            return MapToEntityDto(invoice);
        }

        //public override async Task<InvoiceDto> CreateAsync(CreateUpdateInvoiceDto input)
        //{
        //    var invoice = ObjectMapper.Map<Invoices.Invoice>(input);

        //    // Generate invoice no
        //    invoice.InvoiceNo = await GenerateInvoiceNoAsync(input.TenantId);

        //    // Save Invoice
        //    invoice.Status = InvoiceStatus.Paid;
        //    await Repository.InsertAsync(invoice);
        //    await CurrentUnitOfWork.SaveChangesAsync();

        //    // Save Invoice Items
        //    foreach (var item in input.Items)
        //    {
        //        var invoiceItem = ObjectMapper.Map<InvoiceItem>(item);
        //        invoiceItem.InvoiceId = invoice.Id;
        //        await Repository.GetDbContext().Set<InvoiceItem>().AddAsync(invoiceItem);
        //    }
        //    await CurrentUnitOfWork.SaveChangesAsync();

        //    // Handle Deposit
        //    var patientDeposit = await _patientDepositRepository.FirstOrDefaultAsync(x =>
        //        x.PatientId == input.PatientId && x.TenantId == input.TenantId);

        //    if (patientDeposit == null)
        //    {
        //        throw new UserFriendlyException("Patient Deposit record not found.");
        //    }

        //    var transaction = new DepositTransaction
        //    {
        //        TenantId = input.TenantId,
        //        PatientDepositId = patientDeposit.Id,
        //        Amount = input.TotalAmount,
        //        TransactionType = TransactionType.Debit,
        //        PaymentMethod = null,
        //        TransactionDate = DateTime.Now,
        //        Description = $"Invoice {invoice.InvoiceNo}",
        //        ReceiptNo = null,
        //        IsPaid = false
        //    };
        //    await _depositTransactionRepository.InsertAsync(transaction);

        //    patientDeposit.TotalDebitAmount += input.TotalAmount;
        //    patientDeposit.TotalBalance = patientDeposit.TotalCreditAmount - patientDeposit.TotalDebitAmount;

        //    await _patientDepositRepository.UpdateAsync(patientDeposit);
        //    await CurrentUnitOfWork.SaveChangesAsync();


        //    // ✅ Check & Update Patient EmergencyCharge
        //    var patient = await _patientRepository.FirstOrDefaultAsync(x => x.Id == input.PatientId);
        //    if (patient != null && patient.IsEmergencyCharge)   // <-- ye property tumhare patient entity me honi chahiye
        //    {
        //        patient.IsEmergencyCharge = false;
        //        await _patientRepository.UpdateAsync(patient);
        //    }

        //    // ✅ Mark EmergencyChargeEntries as Processed
        //    var emergencyCharges = await _emergencyChargeEntriesRepository.GetAllListAsync(x =>
        //        x.PatientId == input.PatientId && !x.IsProcessed);

        //    if (emergencyCharges.Any())
        //    {
        //        foreach (var charge in emergencyCharges)
        //        {
        //            charge.IsProcessed = true;
        //            await _emergencyChargeEntriesRepository.UpdateAsync(charge);
        //        }
        //    }

        //    // ✅ Mark IpdChargeEntries as Processed (jo tum already kar rahe ho)
        //    var charges = await GetChargesByPatientAsync(input.PatientId, input.InvoiceType);
        //    if (charges.Any())
        //    {
        //        foreach (var chargeDto in charges)
        //        {
        //            var chargeEntity = await _ipdChargeEntryRepository.GetAsync(chargeDto.Id);
        //            chargeEntity.IsProcessed = true;
        //            await _ipdChargeEntryRepository.UpdateAsync(chargeEntity);
        //        }
        //    }

        //    await CurrentUnitOfWork.SaveChangesAsync();

        //    // Return final DTO
        //    return MapToEntityDto(invoice);
        //}
        //public async Task MarkAsPaid(long invoiceId, decimal? amount = null)
        //{
        //    try
        //    {
        //        var invoice = await Repository.GetAsync(invoiceId);
        //        if (invoice == null)
        //            throw new UserFriendlyException("Invoice not found");

        //        // Determine payment amount
        //        decimal paymentAmount = amount.HasValue
        //            ? amount.Value
        //            : invoice.TotalAmount - invoice.AmountPaid;

        //        // Validate payment amount
        //        if (paymentAmount <= 0)
        //        {
        //            throw new UserFriendlyException(
        //                "Payment amount must be greater than zero");
        //        }

        //        decimal newAmountPaid = invoice.AmountPaid;

        //        // Validate payment doesn't exceed total
        //        if (newAmountPaid > invoice.TotalAmount)
        //        {
        //            throw new UserFriendlyException(
        //                $"Payment amount exceeds total due. Maximum allowed: {invoice.TotalAmount - invoice.AmountPaid:C}");
        //        }

        //        // Update payment information
        //        invoice.AmountPaid = newAmountPaid;

        //        // Update status based on payment
        //        if (invoice.AmountPaid >= invoice.TotalAmount)
        //        {
        //            invoice.Status = InvoiceStatus.Paid;
        //        }
        //        else
        //        {
        //            invoice.Status = InvoiceStatus.PartiallyPaid;
        //        }

        //        await Repository.UpdateAsync(invoice);
        //        await CurrentUnitOfWork.SaveChangesAsync();
        //    }
        //    catch (Exception ex)
        //    {
        //        Logger.Error("Error recording payment", ex);
        //        throw new UserFriendlyException("Error updating payment information");
        //    }
        //}
        //public async Task<List<IpdChargeEntryDto>> GetChargesByPatientAsync(long patientId, InvoiceType invoiceType)
        //{
        //    var query = _ipdChargeEntryRepository
        //                .GetAllIncluding(x => x.Patient, x => x.Admission)
        //                .Where(x => x.PatientId == patientId && !x.IsProcessed);

        //    // 🔥 InvoiceType ke hisaab se filter lagana
        //    if (invoiceType == InvoiceType.DailyInvoice)
        //    {
        //        var today = DateTime.Today;
        //        query = query.Where(x => x.EntryDate.Date == today);
        //    }
        //    // Agar FullInvoice hai to filter nahi lagega (sare charges aayenge)

        //    var charges = query.ToList(); // execute query

        //    return ObjectMapper.Map<List<IpdChargeEntryDto>>(charges);
        //}

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
                .Include(x => x.InsuranceClaims).ThenInclude(pi => pi.PatientInsurance).ThenInclude(i => i.InsuranceMaster)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (invoice == null)
            {
                throw new Abp.UI.UserFriendlyException("Invoice not found");
            }

            return ObjectMapper.Map<InvoiceDto>(invoice);
        }
        public async Task<List<InvoiceDto>> GetInvoicesByPatientID(long patientID)
        {
            var list = await Repository.GetAll().Where(x => x.PatientId == patientID).ToListAsync();
            var mappedList = ObjectMapper.Map<List<InvoiceDto>>(list);
            return mappedList;
        }
    }
}
