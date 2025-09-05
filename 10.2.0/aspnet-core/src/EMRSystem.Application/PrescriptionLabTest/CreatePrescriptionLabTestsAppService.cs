using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Domain.Entities;
using Abp.Domain.Repositories;
using Abp.UI;
using AutoMapper.Internal.Mappers;
using EMRSystem.Invoices;
using EMRSystem.IpdChargeEntry;
using EMRSystem.LabReport.Dto;
using EMRSystem.LabReports;
using EMRSystem.LabTestReceipt;
using EMRSystem.Patients;
using EMRSystem.PrescriptionLabTest.Dto;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Stripe;
using Stripe.Checkout;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PaymentMethod = EMRSystem.Invoices.PaymentMethod;

namespace EMRSystem.PrescriptionLabTest
{
   
    public class CreatePrescriptionLabTestsAppService : AsyncCrudAppService<EMRSystem.LabReports.PrescriptionLabTest, 
        PrescriptionLabTestDto, long, PagedAndSortedResultRequestDto, CreateUpdatePrescriptionLabTestDto, 
        CreateUpdatePrescriptionLabTestDto>, ICreatePrescriptionLabTestsAppService
    {
        private readonly IRepository<Patient, long> _patientRepository;
        private readonly IRepository<EMRSystem.LabReportsTypes.LabReportsType, long> _labReportsTypeRepository;
        private readonly IRepository<EMRSystem.IpdChargeEntry.IpdChargeEntry, long> _ipdChargeEntryRepository;
        private readonly IConfiguration _configuration;
        private readonly IRepository<LabReportResultItem, long> _resultItemRepo;
        private readonly ILabTestReceiptAppService _labTestReceiptAppService;

        public CreatePrescriptionLabTestsAppService(
            IRepository<EMRSystem.LabReports.PrescriptionLabTest, long> repository,
            IRepository<Patient, long> patientRepository,
            IRepository<LabReportResultItem, long> resultItemRepository,
            IRepository<EMRSystem.LabReportsTypes.LabReportsType, long> labReportsTypeRepository,
            IRepository<EMRSystem.IpdChargeEntry.IpdChargeEntry, long> ipdChargeEntryRepository,
            IConfiguration configuration,
            ILabTestReceiptAppService labTestReceiptAppService)
            : base(repository)
        {
            _patientRepository = patientRepository;
            _labReportsTypeRepository = labReportsTypeRepository;
            _resultItemRepo = resultItemRepository;
            _ipdChargeEntryRepository = ipdChargeEntryRepository;
            _configuration = configuration;
            _labTestReceiptAppService = labTestReceiptAppService;
        }

        //public async Task<LabTestCreationResultDto> CreateLabTestAsync(CreateUpdatePrescriptionLabTestDto input)
        //{
        //    var patient = await _patientRepository.GetAllIncluding(p => p.Admissions)
        //        .FirstOrDefaultAsync(p => p.Id == input.PatientId);

        //    if (patient == null)
        //        throw new UserFriendlyException("Patient not found");

        //    var labTestType = await _labReportsTypeRepository.GetAsync(input.LabReportsTypeId);
        //    if (labTestType == null)
        //        throw new UserFriendlyException("Lab test type not found");

        //    var entity = ObjectMapper.Map<EMRSystem.LabReports.PrescriptionLabTest>(input);
        //    entity.TestStatus = LabTestStatus.InProgress;

        //    if (patient.IsAdmitted)
        //    {
        //        // IPD Patient
        //        if (!patient.Admissions.Any())
        //            throw new UserFriendlyException("Patient is marked as admitted but has no admission records");

        //        var admission = patient.Admissions
        //            .Where(a => !a.IsDischarged)
        //            .OrderByDescending(a => a.AdmissionDateTime)
        //            .FirstOrDefault()
        //            ?? throw new UserFriendlyException("No active admission found for this patient");

        //        if (admission == null)
        //            throw new UserFriendlyException("No active admission found for patient");

        //        entity.IsPaid = true;
        //        await Repository.InsertAsync(entity);
        //        await CurrentUnitOfWork.SaveChangesAsync();
        //        await SaveResultItemsAsync(entity.Id, input.ResultItems);

        //        var chargeEntry = new EMRSystem.IpdChargeEntry.IpdChargeEntry
        //        {
        //            AdmissionId = admission.Id,
        //            PatientId = patient.Id,
        //            ChargeType = ChargeType.LabTest,
        //            Description = $"Lab Test - {labTestType.ReportType}",
        //            Amount = labTestType.ReportPrice,
        //            //ReferenceId = entity.Id
        //        };

        //        await _ipdChargeEntryRepository.InsertAsync(chargeEntry);

        //        return new LabTestCreationResultDto
        //        {
        //            IsStripeRedirect = false,
        //            Message = "Lab test created. Charge will be deducted from deposit."
        //        };
        //    }
        //    else
        //    {
        //        // OPD Patient
        //        entity.IsPaid = (input.PaymentMethod != PaymentMethod.Card);
        //        await Repository.InsertAsync(entity);
        //        await CurrentUnitOfWork.SaveChangesAsync();
        //        await SaveResultItemsAsync(entity.Id, input.ResultItems);

        //        if (input.PaymentMethod == PaymentMethod.Card)
        //        {
        //            var stripeUrl = await CreateStripeSessionForLabTest(
        //                entity,
        //                labTestType.ReportPrice,
        //                "http://localhost:4200/app/lab-technician/test-requests", // Success URL
        //                "http://localhost:4200/app/lab-technician/test-requests"  // Cancel URL
        //            );

        //            return new LabTestCreationResultDto
        //            {
        //                IsStripeRedirect = true,
        //                StripeSessionUrl = stripeUrl
        //            };
        //        }
        //        else // Cash payment
        //        {
        //            var receipt = await _labTestReceiptAppService
        //                .GenerateLabTestReceipt(entity.Id, input.PaymentMethod.Value.ToString());
        //            return new LabTestCreationResultDto
        //            {
        //                IsStripeRedirect = false,
        //                Receipt = receipt
        //            };
        //        }
        //    }
        //}
        private async Task SaveResultItemsAsync(long parentTestId, List<LabReportResultItemDto> dtos)
        {
            if (dtos == null || dtos.Count == 0)
                return;

            foreach (var dto in dtos)
            {
                var entity = new LabReportResultItem
                {
                    PrescriptionLabTestId = parentTestId,
                    Test = dto.Test,
                    Result = dto.Result,
                    MinValue = dto.MinValue,
                    MaxValue = dto.MaxValue,
                    Unit = dto.Unit,
                    Flag = dto.Flag
                };
                await _resultItemRepo.InsertAsync(entity);
            }
        }

        private async Task<string> CreateStripeSessionForLabTest(EMRSystem.LabReports.PrescriptionLabTest labTest, decimal amount, string successUrl, string cancelUrl)
        {
            if (labTest == null)
                throw new ArgumentNullException(nameof(labTest));

            // Pick the correct PatientId: prefer the one on labTest, else from its Prescription
            Patient patientEntity;
            if (labTest.PatientId.HasValue)
            {
                patientEntity = await _patientRepository.GetAsync(labTest.PatientId.Value);
            }
            else if (labTest.Prescription?.PatientId > 0)
            {
                patientEntity = await _patientRepository.GetAsync(labTest.Prescription.PatientId.Value);
            }
            else
            {
                throw new UserFriendlyException("No patient associated with this lab test.");
            }

            var labTestType = await _labReportsTypeRepository.GetAsync(labTest.LabReportsTypeId);

            StripeConfiguration.ApiKey = _configuration["Stripe:SecretKey"]
                ?? throw new ConfigurationException("Stripe:SecretKey is not configured");

            var options = new SessionCreateOptions
            {
                PaymentMethodTypes = new List<string> { "card" },
                LineItems = new List<SessionLineItemOptions>
        {
            new()
            {
                PriceData = new SessionLineItemPriceDataOptions
                {
                    UnitAmount = (long)(amount * 100),
                    Currency = "inr",
                    ProductData = new SessionLineItemPriceDataProductDataOptions
                    {
                        Name = $"Lab Test - {labTestType.ReportType}",
                        Description = $"For patient: {patientEntity.FullName}"
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
            { "labTestId", labTest.Id.ToString() },
            { "tenantId", labTest.TenantId.ToString() },
            { "purpose", "labtest" }
        }
            };

            try
            {
                var service = new SessionService();
                var session = await service.CreateAsync(options);
                return session.Url;
            }
            catch (StripeException ex)
            {
                Logger.Error("Stripe session creation failed for lab test", ex);
                throw new UserFriendlyException("Payment processing error. Please try again.");
            }
        }

        public async Task<string> InitiatePaymentForLabTest(long labTestId)
        {
            // Eager-load Prescription so CreateStripeSession has it available
            var labTest = await Repository
                .GetAllIncluding(lt => lt.Prescription)
                .FirstOrDefaultAsync(lt => lt.Id == labTestId);

            if (labTest == null)
                throw new EntityNotFoundException(typeof(EMRSystem.LabReports.PrescriptionLabTest), labTestId);

            if (labTest.IsPaid)
                throw new UserFriendlyException("Lab test is already paid");

            var labTestType = await _labReportsTypeRepository.GetAsync(labTest.LabReportsTypeId);
            if (labTestType == null)
                throw new UserFriendlyException("Lab test type not found");

            // Pass the fully hydrated labTest into your Stripe helper
            return await CreateStripeSessionForLabTest(
                labTest,
                labTestType.ReportPrice,
                "http://localhost:4200/app/lab-technician/test-requests",
                "http://localhost:4200/app/lab-technician/test-requests"
            );
        }

        public async Task MakeInprogressReport(long id)
        {
            var data = await Repository.GetAsync(id);
            if (data == null) return;

            data.TestStatus = LabTestStatus.InProgress;
            await Repository.UpdateAsync(data);
        }
    }
}
