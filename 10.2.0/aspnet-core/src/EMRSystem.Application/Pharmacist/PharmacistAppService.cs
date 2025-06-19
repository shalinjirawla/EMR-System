using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Domain.Repositories;
using EMRSystem.Authorization;
using EMRSystem.Nurse;
using EMRSystem.Nurse.Dto;
using EMRSystem.Pharmacist.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMRSystem.Pharmacist
{
    [AbpAuthorize(PermissionNames.Pages_Pharmacist)]
    public class PharmacistAppService : AsyncCrudAppService<EMRSystem.Pharmacists.Pharmacist, PharmacistDto, long, PagedAndSortedResultRequestDto, CreateUpdatePharmacistDto, CreateUpdatePharmacistDto>,
     IPharmacistAppService
    {
        public PharmacistAppService(IRepository<EMRSystem.Pharmacists.Pharmacist, long> repository) : base(repository)
        {
        }
    }
}
