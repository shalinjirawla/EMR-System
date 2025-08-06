using Abp.Domain.Entities;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using EMRSystem.Invoices;
using EMRSystem.LabTestReceipt.Dto;
using EMRSystem.Patients;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMRSystem.LabTestReceipt
{
    public class LabTestReceiptAppService : ILabTestReceiptAppService
    {
        private readonly IRepository<LabTestReceipt, long> _receiptRepository;
        private readonly IRepository<EMRSystem.LabReports.PrescriptionLabTest, long> _labTestRepository;
        private readonly IRepository<Patient, long> _patientRepository;
        private readonly IRepository<EMRSystem.LabReportsTypes.LabReportsType, long> _labTypeRepository;
        private readonly IUnitOfWorkManager _unitOfWorkManager;

        public LabTestReceiptAppService(
            IRepository<LabTestReceipt, long> receiptRepository,
            IRepository<EMRSystem.LabReports.PrescriptionLabTest, long> labTestRepository,
            IRepository<Patient, long> patientRepository,
            IUnitOfWorkManager unitOfWorkManager,
            IRepository<EMRSystem.LabReportsTypes.LabReportsType, long> labTypeRepository)
        {
            _receiptRepository = receiptRepository;
            _labTestRepository = labTestRepository;
            _patientRepository = patientRepository;
            _labTypeRepository = labTypeRepository;
            _unitOfWorkManager = unitOfWorkManager;
        }

        //[UnitOfWork]
        public async Task<LabTestReceiptDto> GenerateLabTestReceipt(long labTestId, string paymentMethod)
        {
            var labTest = await _labTestRepository.GetAll()
                .Include(lt => lt.Patient)
                .Include(lt => lt.LabReportsType)
                .FirstOrDefaultAsync(lt => lt.Id == labTestId);

            if (labTest == null)
                throw new EntityNotFoundException(typeof(EMRSystem.LabReports.PrescriptionLabTest), labTestId);

            var receipt = new LabTestReceipt
            {
                TenantId = labTest.TenantId,
                PatientId = labTest.PatientId.Value,
                LabReportTypeId = labTest.LabReportsTypeId,
                LabTestFee = labTest.LabReportsType.ReportPrice,
                PaymentMethod = (PaymentMethod)Enum.Parse(typeof(PaymentMethod), paymentMethod),
                ReceiptNumber = await GenerateReceiptNumberAsync(),
                Status = InvoiceStatus.Paid,
                PaymentDate = DateTime.Now
            };

            await _receiptRepository.InsertAsync(receipt);
            await _unitOfWorkManager.Current.SaveChangesAsync();

            // Return DTO with additional info
            return new LabTestReceiptDto
            {
                Id = receipt.Id,
                TenantId = receipt.TenantId,
                PatientId = receipt.PatientId,
                LabReportTypeId = receipt.LabReportTypeId,
                LabTestFee = receipt.LabTestFee,
                ReceiptNumber = receipt.ReceiptNumber,
                PaymentDate = receipt.PaymentDate,
                PaymentMethod = receipt.PaymentMethod,
                Status = receipt.Status,
                PatientName = labTest.Patient?.FullName,
                LabReportTypeName = labTest.LabReportsType?.ReportType
            };
        }

        private async Task<string> GenerateReceiptNumberAsync()
        {
            var today = DateTime.Today.ToString("yyyyMMdd");
            var lastReceipt = await _receiptRepository.GetAll()
                .Where(r => r.ReceiptNumber.StartsWith($"LABREC-{today}"))
                .OrderByDescending(r => r.ReceiptNumber)
                .FirstOrDefaultAsync();

            int sequence = 1;
            if (lastReceipt != null)
            {
                var parts = lastReceipt.ReceiptNumber.Split('-');
                if (parts.Length == 3 && int.TryParse(parts[2], out int lastSeq))
                {
                    sequence = lastSeq + 1;
                }
            }

            return $"LABREC-{today}-{sequence.ToString().PadLeft(3, '0')}";
        }
    }
}
