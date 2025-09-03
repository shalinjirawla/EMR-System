using Abp.Application.Services.Dto;
using Abp.Application.Services;
using EMRSystem.Pharmacist.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMRSystem.Pharmacist
{
    public interface IPharmacistPrescriptionsAppService : IAsyncCrudAppService<
    PharmacistPrescriptionsDto, long, PagedAndSortedResultRequestDto, CreateUpdatePharmacistPrescriptionsDto, CreateUpdatePharmacistPrescriptionsDto>
    {
    }
}
