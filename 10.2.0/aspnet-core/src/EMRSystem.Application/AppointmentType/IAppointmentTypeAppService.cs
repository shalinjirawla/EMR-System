using Abp.Application.Services;
using Abp.Application.Services.Dto;
using EMRSystem.AppointmentType.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMRSystem.AppointmentType
{
    public interface IAppointmentTypeAppService : IAsyncCrudAppService<
        AppointmentTypeDto, long, PagedResultRequestDto, CreateUpdateAppointmentTypeDto, CreateUpdateAppointmentTypeDto>
    {
        Task<ListResultDto<AppointmentTypeDto>> GetAllForTenant();
    }
}
