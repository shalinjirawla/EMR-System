using Abp.Application.Services.Dto;
using Abp.Application.Services;
using EMRSystem.Prescriptions.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EMRSystem.Vitals.Dto;

namespace EMRSystem.Vitals
{
    public interface IVitalAppService : IAsyncCrudAppService<
    VitalDto, long, PagedAndSortedResultRequestDto, CreateUpdateVitalDto, CreateUpdateVitalDto>
    {
    }
}
