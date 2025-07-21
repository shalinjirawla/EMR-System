using Abp.Application.Services;
using Abp.Application.Services.Dto;
using EMRSystem.DoctorMaster.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMRSystem.DoctorMaster
{
    public interface IDoctorMasterAppService : IAsyncCrudAppService<
        DoctorMasterDto, long, PagedDoctorMasterResultRequestDto,
        CreateUpdateDoctorMasterDto, CreateUpdateDoctorMasterDto>
    {
        Task<ListResultDto<DoctorMasterDto>> GetAllByTenantIdAsync(int tenantId);
    }
}
