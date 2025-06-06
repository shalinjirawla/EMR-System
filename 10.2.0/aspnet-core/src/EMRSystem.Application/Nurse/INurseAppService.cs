using Abp.Application.Services.Dto;
using Abp.Application.Services;
using EMRSystem.Doctor.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EMRSystem.Nurse.Dto;

namespace EMRSystem.Nurse
{
    public interface INurseAppService : IAsyncCrudAppService<
    NurseDto, long, PagedAndSortedResultRequestDto, CreateUpdateNurseDto, CreateUpdateNurseDto>
    {
    }
}
