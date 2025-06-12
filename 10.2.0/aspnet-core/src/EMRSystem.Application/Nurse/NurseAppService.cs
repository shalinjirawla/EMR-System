using Abp.Application.Services.Dto;
using Abp.Application.Services;
using Abp.Domain.Repositories;
using EMRSystem.Doctor.Dto;
using EMRSystem.Doctor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EMRSystem.Nurse.Dto;
using Abp.Authorization;
using EMRSystem.Authorization;

namespace EMRSystem.Nurse
{
    //[AbpAuthorize(PermissionNames.Pages_Nurses)]
    public class NurseAppService : AsyncCrudAppService<EMRSystem.Nurses.Nurse, NurseDto, long, PagedAndSortedResultRequestDto, CreateUpdateNurseDto, CreateUpdateNurseDto>,
   INurseAppService
    {
        public NurseAppService(IRepository<EMRSystem.Nurses.Nurse, long> repository) : base(repository)
        {
        }
    }
}
