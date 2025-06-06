using Abp.Application.Services.Dto;
using Abp.Application.Services;
using Abp.Domain.Repositories;
using EMRSystem.Billings;
using EMRSystem.BillingStaff.Dto;
using EMRSystem.BillingStaff;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EMRSystem.Doctors;
using EMRSystem.Doctor.Dto;
using Abp.Authorization;
using EMRSystem.Authorization;

namespace EMRSystem.Doctor
{
    [AbpAuthorize(PermissionNames.Pages_Doctors)]
    public class DoctorAppService : AsyncCrudAppService<EMRSystem.Doctors.Doctor, DoctorDto, long, PagedAndSortedResultRequestDto, CreateUpdateDoctorDto, CreateUpdateDoctorDto>,
   IDoctorAppService
    {
        public DoctorAppService(IRepository<EMRSystem.Doctors.Doctor, long> repository) : base(repository)
        {
        }
    }
}
