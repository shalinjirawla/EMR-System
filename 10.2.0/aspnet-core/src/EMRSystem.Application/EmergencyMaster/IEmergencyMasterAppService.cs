using Abp.Application.Services.Dto;
using Abp.Application.Services;
using EMRSystem.DoctorMaster.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EMRSystem.EmergencyMaster.Dto;
using EMRSystem.Users.Dto;

namespace EMRSystem.EmergencyMaster
{
    public interface IEmergencyMasterAppService : IAsyncCrudAppService<
        EmergencyMasterDto, long, PagedResultRequestDto,
        CreateUpdateEmergencyMasterDto, CreateUpdateEmergencyMasterDto>
    {
    }
}
