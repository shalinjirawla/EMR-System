using Abp.Application.Services;
using Abp.Domain.Repositories;
using Abp.EntityFrameworkCore.Repositories;
using Abp.Extensions;
using Abp.UI;
using EMRSystem.Appointments;
using EMRSystem.Deposit;
using EMRSystem.EmergencyChargeEntries;
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
        private readonly IRepository<PatientDeposit, long> _patientDepositRepository;
        private readonly IRepository<Patient, long> _patientRepository;
        private readonly IRepository<DepositTransaction, long> _depositTransactionRepository;
        private readonly IRepository<EMRSystem.IpdChargeEntry.IpdChargeEntry, long> _ipdChargeEntryRepository;
        private readonly IRepository<EMRSystem.EmergencyChargeEntries.EmergencyChargeEntry, long> _emergencyChargeEntriesRepository;
        public InvoiceAppService(
            IRepository<EMRSystem.Invoices.Invoice, long> repository,
            INumberingService numberingService,
            IRepository<Patient, long> patientRepository,
            IRepository<PatientDeposit, long> patientDepositRepository,
            IRepository<DepositTransaction, long> depositTransactionRepository,
            IRepository<EMRSystem.EmergencyChargeEntries.EmergencyChargeEntry, long> emergencyChargeEntriesRepository,
            IConfiguration configuration,
            IRepository<EMRSystem.IpdChargeEntry.IpdChargeEntry, long> ipdChargeEntryRepository) : base(repository)
        {
            _configuration = configuration;
            _numberingService = numberingService;
            _patientRepository = patientRepository;
            _patientDepositRepository = patientDepositRepository;
            _depositTransactionRepository = depositTransactionRepository;
            _ipdChargeEntryRepository = ipdChargeEntryRepository;
            _emergencyChargeEntriesRepository = emergencyChargeEntriesRepository;
            StripeConfiguration.ApiKey = _configuration["Stripe:SecretKey"];
        }

        protected override IQueryable<EMRSystem.Invoices.Invoice> CreateFilteredQuery(PagedInvoiceResultRequestDto input)
        {
            // Start with base query including related entities
            var baseQuery = Repository
                .GetAll()
                .Include(x => x.Patient);

            var filteredQuery = baseQuery.AsQueryable();

            if (!input.Keyword.IsNullOrWhiteSpace())
            {
                filteredQuery = filteredQuery.Where(x =>
                    (x.Patient.FullName != null && x.Patient.FullName.Contains(input.Keyword)));
                //(x.Appointment.ReasonForVisit != null && x.Appointment.ReasonForVisit.Contains(input.Keyword)));
            }

            //if (input.Status.HasValue)
            //{
            //    filteredQuery = filteredQuery.Where(x => x.Status == input.Status.Value);
            //}

            //if (input.PaymentMethod.HasValue)
            //{
            //    filteredQuery = filteredQuery.Where(x => x.PaymentMethod == input.PaymentMethod.Value);
            //}

            //if (input.FromDate.HasValue)
            //{
            //    filteredQuery = filteredQuery.Where(x => x.InvoiceDate >= input.FromDate.Value);
            //}

            //if (input.ToDate.HasValue)
            //{
            //    filteredQuery = filteredQuery.Where(x => x.InvoiceDate <= input.ToDate.Value);
            //}

            // Apply projection after all filtering
            var result = filteredQuery.Select(x => new EMRSystem.Invoices.Invoice
            {
                Id = x.Id,
                TenantId = x.TenantId,
                InvoiceDate = x.InvoiceDate,
                InvoiceNo = x.InvoiceNo,
                InvoiceType = x.InvoiceType,
                //DueDate = x.DueDate,
                SubTotal = x.SubTotal,
                GstAmount = x.GstAmount,
                TotalAmount = x.TotalAmount,
                Status = x.Status,
                PaymentMethod = x.PaymentMethod,
                //AmountPaid=x.AmountPaid,
                PatientId = x.PatientId,
                //AppointmentId = x.AppointmentId,
                Patient = x.Patient == null ? null : new Patient
                {
                    Id = x.Patient.Id,
                    FullName = x.Patient.FullName
                },
                //Appointment = x.Appointment == null ? null : new Appointment
                //{
                //    Id = x.Appointment.Id,
                //    AppointmentDate = x.Appointment.AppointmentDate,
                //    ReasonForVisit = x.Appointment.ReasonForVisit
                //},
                Items = x.Items.Select(i => new InvoiceItem
                {
                    Id = i.Id,
                    Description = i.Description,
                    UnitPrice = i.UnitPrice,
                    Quantity = i.Quantity,
                    //ItemType = i.ItemType
                }).ToList()
            });

            return result;
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

        public async Task<InvoiceDetailsDto> GetInvoiceDetailsByAppointmentIdUsingSp(long appointmentId)
        {
            try
            {
                // Create connection and command
                await using var connection = new SqlConnection(Repository.GetDbContext().Database.GetDbConnection().ConnectionString);
                await connection.OpenAsync();

                var command = new SqlCommand("sp_GetInvoiceDetailsByAppointmentId", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };
                command.Parameters.Add(new SqlParameter("@AppointmentId", appointmentId));

                // Execute the reader
                await using var reader = await command.ExecuteReaderAsync();

                // 1. Read the JSON results
                var result = new
                {
                    AppointmentDetails = "",
                    MedicineDetails = "",
                    LabTestDetails = ""
                };

                if (await reader.ReadAsync())
                {
                    result = new
                    {
                        AppointmentDetails = reader.GetString(0),
                        MedicineDetails = reader.GetString(1),
                        LabTestDetails = reader.GetString(2)
                    };
                }
                else
                {
                    throw new UserFriendlyException("No invoice details found");
                }

                // 2. Parse the JSON data
                var appointment = JsonConvert.DeserializeObject<AppointmentDetails>(result.AppointmentDetails);
                var medicines = JsonConvert.DeserializeObject<List<MedicineDetailDto>>(result.MedicineDetails ?? "[]");
                var labTests = JsonConvert.DeserializeObject<List<LabTestDetailDto>>(result.LabTestDetails ?? "[]");

                // 3. Map to InvoiceDetailsDto
                var invoiceDetails = new InvoiceDetailsDto
                {
                    AppointmentId = appointment.AppointmentId,
                    AppointmentDate = appointment.AppointmentDate,
                    PatientName = appointment.PatientName,
                    DoctorName = appointment.DoctorName,
                    ConsultationFee = appointment.ConsultationFee,
                    Medicines = medicines,
                    LabTests = labTests
                };

                // Calculate totals
                CalculateInvoiceTotals(invoiceDetails);

                return invoiceDetails;
            }
            catch (JsonException jsonEx)
            {
                Logger.Error("JSON parsing error in GetInvoiceDetailsByAppointmentIdUsingSp", jsonEx);
                throw new UserFriendlyException("Error processing invoice data format");
            }
            catch (SqlException sqlEx)
            {
                Logger.Error("SQL Error in GetInvoiceDetailsByAppointmentIdUsingSp", sqlEx);
                throw new UserFriendlyException("Database error occurred while fetching invoice details", sqlEx.Message);
            }
            catch (UserFriendlyException)
            {
                throw;
            }
            catch (Exception ex)
            {
                Logger.Error("Unexpected error in GetInvoiceDetailsByAppointmentIdUsingSp", ex);
                throw new UserFriendlyException("An error occurred while processing your request");
            }
        }

        private void CalculateInvoiceTotals(InvoiceDetailsDto dto)
        {
            try
            {
                decimal subtotal = dto.ConsultationFee;
                subtotal += dto.LabTests?.Sum(lt => lt.Price) ?? 0;
                subtotal += dto.Medicines?.Sum(m => m.Price * m.Quantity) ?? 0;

                decimal gst = subtotal * 0.18m;

                dto.SubTotal = subtotal;
                dto.GstAmount = gst;
                dto.TotalAmount = subtotal + gst;
            }
            catch (Exception ex)
            {
                Logger.Error("Error in CalculateInvoiceTotals", ex);
                dto.SubTotal = 0;
                dto.GstAmount = 0;
                dto.TotalAmount = 0;
            }
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
            var invoice = ObjectMapper.Map<Invoices.Invoice>(input);

            // Generate invoice no
            invoice.InvoiceNo = await GenerateInvoiceNoAsync(input.TenantId);

            // Save Invoice
            invoice.Status = InvoiceStatus.Paid;
            await Repository.InsertAsync(invoice);
            await CurrentUnitOfWork.SaveChangesAsync();

            // Save Invoice Items
            foreach (var item in input.Items)
            {
                var invoiceItem = ObjectMapper.Map<InvoiceItem>(item);
                invoiceItem.InvoiceId = invoice.Id;
                await Repository.GetDbContext().Set<InvoiceItem>().AddAsync(invoiceItem);
            }
            await CurrentUnitOfWork.SaveChangesAsync();

            // Handle Deposit
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


            // ✅ Check & Update Patient EmergencyCharge
            var patient = await _patientRepository.FirstOrDefaultAsync(x => x.Id == input.PatientId);
            if (patient != null && patient.IsEmergencyCharge)   // <-- ye property tumhare patient entity me honi chahiye
            {
                patient.IsEmergencyCharge = false;
                await _patientRepository.UpdateAsync(patient);
            }

            // ✅ Mark EmergencyChargeEntries as Processed
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

            // ✅ Mark IpdChargeEntries as Processed (jo tum already kar rahe ho)
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

            // Return final DTO
            return MapToEntityDto(invoice);
        }

        private void CalculateInvoiceTotals(CreateUpdateInvoiceDto input, EMRSystem.Invoices.Invoice invoice)
        {
            decimal subtotal = input.Items.Sum(item => item.UnitPrice * item.Quantity);
            decimal gstAmount = subtotal * 0.18m; // 18% GST

            invoice.SubTotal = subtotal;
            invoice.GstAmount = gstAmount;
            invoice.TotalAmount = subtotal + gstAmount;
        }
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

            var today = DateTime.Today;

            // 🔹 IPD Charges
            var ipdQuery = _ipdChargeEntryRepository
                .GetAllIncluding(x => x.Patient, x => x.Admission)
                .Where(x => x.PatientId == patientId && !x.IsProcessed);

            if (invoiceType == InvoiceType.DailyInvoice)
            {
                ipdQuery = ipdQuery.Where(x => x.EntryDate.Date == today);
            }

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
                .FirstOrDefaultAsync(x => x.Id == id);

            if (invoice == null)
            {
                throw new Abp.UI.UserFriendlyException("Invoice not found");
            }

            return ObjectMapper.Map<InvoiceDto>(invoice);
        }
        public async Task<string> CreateStripeCheckoutSession(long invoiceId, decimal amount, string successUrl, string cancelUrl)
        {
            try
            {
                if (amount <= 0)
                {
                    throw new UserFriendlyException("Payment amount must be greater than zero");
                }
                var options = new SessionCreateOptions
                {
                    PaymentMethodTypes = new List<string> { "card" },
                    LineItems = new List<SessionLineItemOptions>
            {
                new SessionLineItemOptions
                {
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        UnitAmount = (long)(amount * 100),
                        Currency = "inr",
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = $"Invoice #{invoiceId}",
                             Description = "Partial payment for medical services"
                        },
                    },
                    Quantity = 1,
                },
            },
                    Mode = "payment",
                    SuccessUrl = successUrl,
                    CancelUrl = cancelUrl,
                    Metadata = new Dictionary<string, string>
            {
                { "invoiceId", invoiceId.ToString() },
                { "paymentType", amount > 0 ? "partial" : "full" }
            }
                };

                var service = new SessionService();
                var session = await service.CreateAsync(options);
                return session.Url;
            }
            catch (StripeException e)
            {
                throw new UserFriendlyException("Stripe checkout creation failed: " + e.Message);
            }
        }
    }
}
