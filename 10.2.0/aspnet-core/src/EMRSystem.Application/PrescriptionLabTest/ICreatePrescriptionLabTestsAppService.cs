using Abp.Application.Services;
using Abp.Application.Services.Dto;
using EMRSystem.PrescriptionLabTest.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMRSystem.PrescriptionLabTest
{
    public interface ICreatePrescriptionLabTestsAppService : IAsyncCrudAppService<PrescriptionLabTestDto, long, PagedAndSortedResultRequestDto, CreateUpdatePrescriptionLabTestDto, CreateUpdatePrescriptionLabTestDto>
    {
        Task<LabTestCreationResultDto> CreateLabTestAsync(CreateUpdatePrescriptionLabTestDto input);
        Task MakeInprogressReport(long id);
        Task<string> InitiatePaymentForLabTest(long labTestId);
    }
}
