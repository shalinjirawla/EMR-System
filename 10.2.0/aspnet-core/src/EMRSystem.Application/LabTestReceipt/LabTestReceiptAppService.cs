using Abp.Application.Services;
using Abp.Domain.Entities;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.Timing;
using Abp.UI;
using AutoMapper;
using EMRSystem.Invoices;
using EMRSystem.LabMasters;
using EMRSystem.LabReports;
using EMRSystem.LabTestReceipt.Dto;
using EMRSystem.Patients;
using EMRSystem.PrescriptionLabTest.Dto;
using EMRSystem.Prescriptions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMRSystem.LabTestReceipt
{
    public class LabTestReceiptAppService : ApplicationService,ILabTestReceiptAppService
    {
        private readonly IRepository<LabTestReceipt, long> _labTestReceiptRepository;
        private readonly IRepository<EMRSystem.LabReports.PrescriptionLabTest, long> _prescriptionLabTestRepository;
        private readonly IRepository<EMRSystem.LabReportsTypes.LabReportsType, long> _labReportsTypeRepository;
        private readonly IRepository<Prescription, long> _prescriptionRepository;
        private readonly IRepository<HealthPackageLabReportsType, long> _packageTestRepository;

        public LabTestReceiptAppService(
        IRepository<LabTestReceipt, long> labTestReceiptRepository,
        IRepository<EMRSystem.LabReports.PrescriptionLabTest, long> prescriptionLabTestRepository,
        IRepository<EMRSystem.LabReportsTypes.LabReportsType, long> labReportsTypeRepository,
        IRepository<Prescription, long> prescriptionRepository,
        IRepository<HealthPackageLabReportsType, long> packageTestRepository)
        {
            _labTestReceiptRepository = labTestReceiptRepository;
            _prescriptionLabTestRepository = prescriptionLabTestRepository;
            _labReportsTypeRepository = labReportsTypeRepository;
            _prescriptionRepository = prescriptionRepository;
            _packageTestRepository = packageTestRepository;
        }

        //[UnitOfWork]
        public async Task<long> CreateLabTestReceipt(CreateLabTestReceiptDto input)
        {
            try
            {
                // 1. VALIDATION
                if (input.LabTestSource == LabTestSource.OPD && !input.PrescriptionId.HasValue)
                {
                    throw new UserFriendlyException("Prescription ID is required for OPD cases");
                }

                // 2. CREATE RECEIPT
                var receipt = new LabTestReceipt
                {
                    TenantId = AbpSession.TenantId.Value,
                    PatientId = input.PatientId,
                    Source = input.LabTestSource,
                    TotalFee = input.TotalAmount,
                    PaymentMethod =  input.PaymentMethod,
                    Status = InvoiceStatus.Paid,
                    ReceiptNumber = GenerateReceiptNumber(),
                    PaymentDate = Clock.Now
                };

                await _labTestReceiptRepository.InsertAsync(receipt);
                await CurrentUnitOfWork.SaveChangesAsync();

                // 3. GET PRESCRIBED TESTS (FOR OPD CASES)
                List<long> prescribedTestIds = new List<long>();
                if (input.LabTestSource == LabTestSource.OPD)
                {
                    prescribedTestIds = await _prescriptionLabTestRepository.GetAll()
                        .Where(x => x.PrescriptionId == input.PrescriptionId)
                        .Select(x => x.LabReportsTypeId)
                        .ToListAsync();
                }

                // 4. PROCESS PACKAGES
                foreach (var packageId in input.SelectedPackageIds)
                {
                    var packageTests = await _packageTestRepository.GetAll()
                        .Where(x => x.HealthPackageId == packageId)
                        .Select(x => x.LabReportsTypeId)
                        .ToListAsync();

                    foreach (var testId in packageTests)
                    {
                        await CreateOrUpdateTestEntry(
                            testId,
                            receipt.Id,
                            input.PatientId,
                            input.LabTestSource,
                            input.PrescriptionId,
                            isFromPackage: true,
                            packageId,
                            prescribedTestIds.Contains(testId)
                        );
                    }
                }

                // 5. PROCESS SINGLE TESTS
                foreach (var testId in input.SelectedTestIds)
                {
                    await CreateOrUpdateTestEntry(
                        testId,
                        receipt.Id,
                        input.PatientId,
                        input.LabTestSource,
                        input.PrescriptionId,
                        isFromPackage: false,
                        packageId: null,
                        prescribedTestIds.Contains(testId)
                    );
                }

                return receipt.Id;
            }
            catch (Exception ex)
            {
                Logger.Error("Error creating lab receipt", ex);
                throw;
            }
        }

        private async Task CreateOrUpdateTestEntry(
            long testId,
            long receiptId,
            long patientId,
            LabTestSource source,
            long? prescriptionId,
            bool isFromPackage,
            long? packageId,
            bool isPrescribed)
        {
            var existingEntry = await _prescriptionLabTestRepository.FirstOrDefaultAsync(
                x => x.PrescriptionId == prescriptionId &&
                     x.LabReportsTypeId == testId &&
                     x.LabTestReceiptId == null);

            if (existingEntry != null)
            {
                // UPDATE EXISTING ENTRY
                existingEntry.LabTestReceiptId = receiptId;
                existingEntry.IsPaid = true;
                existingEntry.IsFromPackage = isFromPackage;
                existingEntry.HealthPackageId = packageId;
                await _prescriptionLabTestRepository.UpdateAsync(existingEntry);
            }
            else
            {
                // CREATE NEW ENTRY
                await _prescriptionLabTestRepository.InsertAsync(new EMRSystem.LabReports.PrescriptionLabTest
                {
                    TenantId = AbpSession.TenantId.Value,
                    LabReportsTypeId = testId,
                    LabTestReceiptId = receiptId,
                    PatientId = patientId,
                    PrescriptionId = source == LabTestSource.OPD && isPrescribed
                        ? prescriptionId
                        : null,
                    TestStatus = LabTestStatus.Pending,
                    CreatedDate = Clock.Now,
                    IsPaid = true,
                    IsPrescribed = isPrescribed,
                    IsFromPackage = isFromPackage,
                    HealthPackageId = packageId
                });
            }
        }

        private string GenerateReceiptNumber()
        {
            return $"LT-{Clock.Now:yyyyMMdd}-{new Random().Next(1000, 9999)}";
        }
    }
}
