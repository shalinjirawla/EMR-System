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
using EMRSystem.Authorization.Users;
using EMRSystem.Users.Dto;
using EMRSystem.Users;
using Abp.UI;
using Microsoft.AspNetCore.Identity;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Net.Mail;
using Castle.Core.Resource;
using EMRSystem.Authorization.Roles;
using EMRSystem.Patients.Dto;
using EMRSystem.Patients;
using Abp.IdentityFramework;

namespace EMRSystem.Doctor
{
    //[AbpAuthorize(PermissionNames.Pages_Doctors)]
    public class DoctorAppService : AsyncCrudAppService<EMRSystem.Doctors.Doctor, DoctorDto, long, PagedAndSortedResultRequestDto, CreateUpdateDoctorDto, CreateUpdateDoctorDto>,
   IDoctorAppService
    {
        private readonly IRepository<EMRSystem.Doctors.Doctor, long> _doctorRepository;
        public DoctorAppService(
            IRepository<EMRSystem.Doctors.Doctor, long> doctorRepository) : base(doctorRepository)
        {
        }

        public async Task<ListResultDto<DoctorDto>> GetAllDoctorsByTenantID(int tenantId)
        {
            var doctorDto = await Repository.GetAllIncludingAsync(x => x.AbpUser);
            var list = doctorDto.Where(x => x.TenantId == tenantId && !x.AbpUser.IsDeleted);
            var mapped = ObjectMapper.Map<List<DoctorDto>>(list);
            var resultList = new ListResultDto<DoctorDto>(mapped);
            return resultList;
        }
    }
}
