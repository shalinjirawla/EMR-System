using Abp.Application.Services;
using Abp.Application.Services.Dto;
using EMRSystem.Nurse.Dto;
using EMRSystem.Pharmacist.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMRSystem.Pharmacist
{
    public interface IPharmacistAppService : IAsyncCrudAppService<
    PharmacistDto, long, PagedAndSortedResultRequestDto, CreateUpdatePharmacistDto, CreateUpdatePharmacistDto>
    {
    }
}
