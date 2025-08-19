using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Domain.Entities;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.Extensions;
using Abp.Linq.Extensions;
using Abp.Timing;
using Abp.UI;
using AutoMapper;
using EMRSystem.Invoices;
using EMRSystem.LabMasters;
using EMRSystem.LabReports;
using EMRSystem.LabTestReceipt.Dto;
using EMRSystem.Patients;
using EMRSystem.Patients.Dto;
using EMRSystem.PrescriptionLabTest.Dto;
using EMRSystem.Prescriptions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Stripe;
using Stripe.Checkout;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PaymentMethod = EMRSystem.Invoices.PaymentMethod;

namespace EMRSystem.LabTestReceipt
{
    public class LabTestReceiptAppService : AsyncCrudAppService<
         LabTestReceipt,
         LabTestReceiptDto,
         long,
         PagedLabTestReceiptResultRequestDto,
         CreateLabTestReceiptDto, // Custom DTO for creation
         UpdateLabTestReceiptDto>, // Custom DTO for update
         ILabTestReceiptAppService
    {
        private readonly IRepository<EMRSystem.LabReports.PrescriptionLabTest, long> _prescriptionLabTestRepository;
        private readonly IRepository<EMRSystem.LabReportsTypes.LabReportsType, long> _labReportsTypeRepository;
        private readonly IRepository<Prescription, long> _prescriptionRepository;
        private readonly IRepository<HealthPackageLabReportsType, long> _packageTestRepository;
        private readonly IConfiguration _configuration;

        public LabTestReceiptAppService(
            IRepository<LabTestReceipt, long> repository,
            IRepository<EMRSystem.LabReports.PrescriptionLabTest, long> prescriptionLabTestRepository,
            IRepository<EMRSystem.LabReportsTypes.LabReportsType, long> labReportsTypeRepository,
            IRepository<Prescription, long> prescriptionRepository,
             IConfiguration configuration,
            IRepository<HealthPackageLabReportsType, long> packageTestRepository)
            : base(repository)
        {
            _prescriptionLabTestRepository = prescriptionLabTestRepository;
            _labReportsTypeRepository = labReportsTypeRepository;
            _prescriptionRepository = prescriptionRepository;
            _packageTestRepository = packageTestRepository;
            _configuration = configuration;
        }

        public async Task<object> ProcessLabTestReceiptAsync(CreateLabTestReceiptDto input)
        {
            if (input.PaymentMethod == PaymentMethod.Cash)
            {
                var receiptId = await CreateLabTestReceipt(input);
                return new { success = true, receiptId };
            }
            else if (input.PaymentMethod == PaymentMethod.Card)
            {
                var checkoutUrl = await CreateStripeCheckoutSessionAsync(input);
                return new { success = true, checkoutUrl };
            }

            throw new UserFriendlyException("Invalid payment method");
        }
        public async Task<string> CreateStripeCheckoutSessionAsync(CreateLabTestReceiptDto input)
        {
            StripeConfiguration.ApiKey = _configuration["Stripe:SecretKey"];

            var patient = await _prescriptionRepository.GetAll()
                            .Include(x => x.Patient)
                            .Select(x => x.Patient)
                            .FirstOrDefaultAsync(p => p.Id == input.PatientId);


            var options = new SessionCreateOptions
            {
                PaymentMethodTypes = new List<string> { "card" },
                LineItems = new List<SessionLineItemOptions>
                {
                    new SessionLineItemOptions
                    {
                        PriceData = new SessionLineItemPriceDataOptions
                        {
                            Currency = "inr",
                            UnitAmount = (long)(input.TotalAmount * 100),
                            ProductData = new SessionLineItemPriceDataProductDataOptions
                            {
                                Name = $"Lab Test Payment - {patient?.FullName ?? "Patient"}"
                            }
                        },
                        Quantity = 1
                    }
                },
                Mode = "payment",
                SuccessUrl = "http://localhost:4200/app/lab-technician/lab-receipts",
                CancelUrl = "http://localhost:4200/app/lab-technician/lab-receipts",
                Metadata = new Dictionary<string, string>
                {
                    { "purpose", "labTest" },
                    { "labTestReceiptDtoJson", Newtonsoft.Json.JsonConvert.SerializeObject(input) },
                    { "tenantId", AbpSession.TenantId.ToString() }
                 }
            };

            var service = new SessionService();
            var session = service.Create(options);

            return session.Url;
        }

        [UnitOfWork]
        public async Task<long> CreateLabTestReceipt(CreateLabTestReceiptDto input)
        {
            try
            {
                // Validation
                if (input.LabTestSource == LabTestSource.OPD && !input.PrescriptionId.HasValue)
                {
                    throw new UserFriendlyException("Prescription ID is required for OPD cases");
                }
                if (input.TenantId == 0 && AbpSession.TenantId.HasValue)
                {
                    input.TenantId = AbpSession.TenantId.Value;
                }

                // Create receipt
                var receipt = new LabTestReceipt
                {
                    TenantId = input.TenantId,
                    PatientId = input.PatientId,
                    Source = input.LabTestSource,
                    TotalFee = input.TotalAmount,
                    PaymentMethod = input.PaymentMethod,
                    Status = InvoiceStatus.Paid,
                    ReceiptNumber = GenerateReceiptNumber(),
                    PaymentDate = Clock.Now
                };

                await Repository.InsertAsync(receipt);
                await CurrentUnitOfWork.SaveChangesAsync();

                // Process tests and packages
                await ProcessReceiptItems(input, receipt.Id);

                return receipt.Id;
            }
            catch (Exception ex)
            {
                Logger.Error("Error creating lab receipt", ex);
                throw;
            }
        }

        private async Task ProcessReceiptItems(CreateLabTestReceiptDto input, long receiptId)
        {
            List<long> prescribedTestIds = new List<long>();
            if (input.LabTestSource == LabTestSource.OPD)
            {
                prescribedTestIds = await _prescriptionLabTestRepository.GetAll()
                    .Where(x => x.PrescriptionId == input.PrescriptionId)
                    .Select(x => x.LabReportsTypeId)
                    .ToListAsync();
            }

            // Process packages
            foreach (var packageId in input.SelectedPackageIds)
            {
                var packageTests = await _packageTestRepository.GetAll()
                    .Include(pt => pt.LabReportsType)
                    .Where(x => x.HealthPackageId == packageId)
                    .ToListAsync();

                foreach (var packageTest in packageTests)
                {
                    await CreateOrUpdateTestEntry(
                        packageTest.LabReportsTypeId,
                        receiptId,
                        input.PatientId,
                        input.LabTestSource,
                        input.PrescriptionId,
                        isFromPackage: true,
                        packageId,
                        prescribedTestIds.Contains(packageTest.LabReportsTypeId),
                        input.TenantId
                    );
                }
            }

            // Process single tests
            foreach (var testId in input.SelectedTestIds)
            {
                var test = await _labReportsTypeRepository.GetAsync(testId);
                await CreateOrUpdateTestEntry(
                    testId,
                    receiptId,
                    input.PatientId,
                    input.LabTestSource,
                    input.PrescriptionId,
                    isFromPackage: false,
                    packageId: null,
                    prescribedTestIds.Contains(testId),
                    input.TenantId
                );
            }
        }

        private async Task CreateOrUpdateTestEntry(long testId,long receiptId,long patientId,LabTestSource source,
            long? prescriptionId,bool isFromPackage,long? packageId,bool isPrescribed, int tenantId)
        {
            var existingEntry = await _prescriptionLabTestRepository.FirstOrDefaultAsync(
                x => x.PrescriptionId == prescriptionId &&
                     x.LabReportsTypeId == testId &&
                     x.LabTestReceiptId == null);

            if (existingEntry != null)
            {
                existingEntry.LabTestReceiptId = receiptId;
                existingEntry.IsPaid = true;
                existingEntry.IsFromPackage = isFromPackage;
                existingEntry.HealthPackageId = packageId;
                await _prescriptionLabTestRepository.UpdateAsync(existingEntry);
            }
            else
            {
                await _prescriptionLabTestRepository.InsertAsync(new EMRSystem.LabReports.PrescriptionLabTest
                {
                    TenantId = AbpSession.TenantId ?? tenantId,
                    LabReportsTypeId = testId,
                    LabTestReceiptId = receiptId,
                    PatientId = patientId,
                    PrescriptionId = source == LabTestSource.OPD && isPrescribed ? prescriptionId : null,
                    TestStatus = LabTestStatus.Pending,
                    CreatedDate = Clock.Now,
                    IsPaid = true,
                    IsPrescribed = isPrescribed,
                    IsFromPackage = isFromPackage,
                    HealthPackageId = packageId
                });
            }
        }

        //public override async Task<LabTestReceiptDto> UpdateAsync(UpdateLabTestReceiptDto input)
        //{
        //    var receipt = await Repository.GetAsync(input.Id);

        //    if (input.PaymentMethod.HasValue)
        //    {
        //        receipt.PaymentMethod = input.PaymentMethod.Value;
        //    }

        //    if (input.Status != InvoiceStatus.None)
        //    {
        //        receipt.Status = input.Status;

        //        // Update related tests if status changed to paid
        //        if (input.Status == InvoiceStatus.Paid)
        //        {
        //            await UpdateRelatedTestsStatus(receipt.Id, true);
        //        }
        //    }

        //    await Repository.UpdateAsync(receipt);
        //    return MapToEntityDto(receipt);
        //}

        //private async Task UpdateRelatedTestsStatus(long receiptId, bool isPaid)
        //{
        //    var tests = await _prescriptionLabTestRepository.GetAll()
        //        .Where(t => t.LabTestReceiptId == receiptId)
        //        .ToListAsync();

        //    foreach (var test in tests)
        //    {
        //        test.IsPaid = isPaid;
        //        if (isPaid) test.TestStatus = LabTestStatus.Pending;
        //    }

        //    await _prescriptionLabTestRepository.UpdateAllAsync(tests);
        //}

        protected override IQueryable<LabTestReceipt> CreateFilteredQuery(PagedLabTestReceiptResultRequestDto input)
        {
            return Repository.GetAll()
                .Include(r => r.Patient)
                .Include(r => r.PrescriptionLabTests)
                    .ThenInclude(plt => plt.LabReportsType)
                .Include(r => r.PrescriptionLabTests)
                    .ThenInclude(plt => plt.HealthPackage)
                .Where(r => r.TenantId == AbpSession.TenantId)
                .WhereIf(!input.Keyword.IsNullOrWhiteSpace(),
                    r => r.ReceiptNumber.Contains(input.Keyword) ||
                         r.Patient.FullName.Contains(input.Keyword));
        }

        protected override LabTestReceiptDto MapToEntityDto(LabTestReceipt entity)
        {
            return ObjectMapper.Map<LabTestReceiptDto>(entity);
        }

        public async Task<ViewLabTestReceiptDto> GetViewByPrescriptionLabTestIdAsync(long prescriptionLabTestId)
        {
            var test = await _prescriptionLabTestRepository.GetAll()
                .Include(t => t.LabTestReceipt)
                .ThenInclude(r => r.Patient)
                .Where(t => t.Id == prescriptionLabTestId)
                .FirstOrDefaultAsync();

            if (test?.LabTestReceipt == null)
                throw new UserFriendlyException("Receipt not found");

            var dto = ObjectMapper.Map<ViewLabTestReceiptDto>(test.LabTestReceipt);
            dto.PatientName = test.LabTestReceipt.Patient.FullName;
            return dto;
        }

        private string GenerateReceiptNumber()
        {
            return $"LT-{Clock.Now:yyyyMMdd}-{new Random().Next(1000, 9999)}";
        }

        public override async Task DeleteAsync(EntityDto<long> input)
        {
            // Check if there are any associated tests
            var hasTests = await _prescriptionLabTestRepository.GetAll()
                .AnyAsync(t => t.LabTestReceiptId == input.Id);

            if (hasTests)
            {
                throw new UserFriendlyException("Cannot delete receipt with associated tests");
            }

            await base.DeleteAsync(input);
        }


        public async Task<LabTestReceiptDisplayDto> GetReceiptDisplayAsync(long receiptId)
        {
            // Load receipt + patient
            var receipt = await Repository.GetAll()
                .Include(r => r.Patient)
                .FirstOrDefaultAsync(r => r.Id == receiptId);

            if (receipt == null)
                throw new UserFriendlyException($"Receipt with id {receiptId} not found.");

            var dto = new LabTestReceiptDisplayDto
            {
                Id = receipt.Id,
                TenantId = receipt.TenantId,
                PatientId = receipt.PatientId,
                Patient = receipt.Patient == null ? null : new PatientDto
                {
                    Id = receipt.Patient.Id,
                    FullName = receipt.Patient.FullName
                    // add MRN if available: MRN = receipt.Patient.MRN
                },
                TotalFee = receipt.TotalFee,
                ReceiptNumber = receipt.ReceiptNumber,
                PaymentDate = receipt.PaymentDate,
                PaymentMethod = receipt.PaymentMethod.ToString(),
                Status = receipt.Status.ToString(),
                Source = receipt.Source.ToString()
            };

            // Query all PrescriptionLabTest entries for this receipt, include related LabReportsType and HealthPackage
            var testsQuery = _prescriptionLabTestRepository.GetAll()
                .Where(t => t.LabTestReceiptId == receiptId)
                .Include(t => t.LabReportsType)
                .Include(t => t.HealthPackage)
                // Order by CreatedDate so we can keep chronological placement
                .OrderBy(t => t.CreatedDate ?? DateTime.MinValue);

            var testsList = await testsQuery.ToListAsync();

            // We'll produce items in order, adding package once at the first time we see it
            var addedPackageIds = new HashSet<long?>();

            foreach (var t in testsList)
            {
                if (t.IsFromPackage)
                {
                    var pkgId = t.HealthPackageId;
                    if (!addedPackageIds.Contains(pkgId))
                    {
                        addedPackageIds.Add(pkgId);

                        // gather all tests that belong to this package (for display)
                        var packageTests = testsList
                            .Where(x => x.IsFromPackage && x.HealthPackageId == pkgId)
                            .Select(x => x.LabReportsType != null ? x.LabReportsType.ReportType : "Unknown Test")
                            .Distinct()
                            .ToList();

                        var packagePrice = t.HealthPackage != null ? t.HealthPackage.PackagePrice : 0m;
                        var packageName = t.HealthPackage != null ? t.HealthPackage.PackageName : "Package";

                        dto.Items.Add(new ReceiptItemDto
                        {
                            IsPackage = true,
                            PackageId = pkgId,
                            PackageName = packageName,
                            PackagePrice = packagePrice,
                            PackageTests = packageTests,
                            CreatedDate = t.CreatedDate
                        });
                    }
                    // else skip, package already added
                }
                else
                {
                    // individual test -> add as its own line
                    dto.Items.Add(new ReceiptItemDto
                    {
                        IsPackage = false,
                        PrescriptionLabTestId = t.Id,
                        LabReportsTypeId = t.LabReportsTypeId,
                        LabReportTypeName = t.LabReportsType != null ? t.LabReportsType.ReportType : "Unknown Test",
                        LabReportPrice = t.LabReportsType != null ? Convert.ToDecimal(t.LabReportsType.ReportPrice) : (decimal?)0,
                        IsPaid = t.IsPaid,
                        TestStatus = (int)t.TestStatus,
                        CreatedDate = t.CreatedDate
                    });
                }
            }

            return dto;
        }
    }
}
