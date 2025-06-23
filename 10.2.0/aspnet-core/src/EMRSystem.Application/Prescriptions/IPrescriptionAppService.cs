using Abp.Application.Services.Dto;
using Abp.Application.Services;
using EMRSystem.BillingStaff.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EMRSystem.Prescriptions.Dto;

namespace EMRSystem.Prescriptions
{
    public interface IPrescriptionAppService : IAsyncCrudAppService<
    PrescriptionDto, long, PagedPrescriptionResultRequestDto, CreateUpdatePrescriptionDto, CreateUpdatePrescriptionDto>
    {
        Task<PrescriptionDto> CreateAsync(CreateUpdatePrescriptionDto input);
    }
}
