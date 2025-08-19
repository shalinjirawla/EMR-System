using Abp.Application.Services;
using EMRSystem.LabTestReceipt.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMRSystem.LabTestReceipt
{
    public interface ILabTestReceiptAppService :
        IAsyncCrudAppService<
            LabTestReceiptDto,
            long,
            PagedLabTestReceiptResultRequestDto,
            CreateLabTestReceiptDto,
            UpdateLabTestReceiptDto>
    {
        Task<long> CreateLabTestReceipt(CreateLabTestReceiptDto input);
        Task<object> ProcessLabTestReceiptAsync(CreateLabTestReceiptDto input);
        Task<LabTestReceiptDisplayDto> GetReceiptDisplayAsync(long receiptId);
        Task<ViewLabTestReceiptDto> GetViewByPrescriptionLabTestIdAsync(long prescriptionLabTestId);
    }
}
