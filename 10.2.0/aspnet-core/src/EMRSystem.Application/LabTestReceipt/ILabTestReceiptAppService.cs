using Abp.Application.Services;
using EMRSystem.LabTestReceipt.Dto;
using EMRSystem.PrescriptionLabTest.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMRSystem.LabTestReceipt
{
    public interface ILabTestReceiptAppService : IApplicationService
    {
        //Task<LabTestReceiptDto> GenerateLabTestReceipt(long labTestId, string paymentMethod);
        Task<long> CreateLabTestReceipt(CreateLabTestReceiptDto input);
        //Task<ViewLabTestReceiptDto> GetViewByPrescriptionLabTestIdAsync(long prescriptionLabTestId);
    }
}
