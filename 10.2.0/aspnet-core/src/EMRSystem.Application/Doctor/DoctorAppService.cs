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
using Abp.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;

namespace EMRSystem.Doctor
{
    //[AbpAuthorize(PermissionNames.Pages_Doctors)]
    public class DoctorAppService : AsyncCrudAppService<EMRSystem.Doctors.Doctor, DoctorDto, long, PagedAndSortedResultRequestDto, CreateUpdateDoctorDto, CreateUpdateDoctorDto>,
   IDoctorAppService
    {
        private readonly UserManager _userManager;
        private readonly IRepository<DoctorMaster.DoctorMaster, long> _doctorMasterRepository;
        public DoctorAppService(
            IRepository<EMRSystem.Doctors.Doctor, long> doctorRepository,
            UserManager userManager,
             IRepository<DoctorMaster.DoctorMaster, long> doctorMasterRepository
            ) : base(doctorRepository)
        {
            _userManager = userManager;
            _doctorMasterRepository = doctorMasterRepository;

        }

        public override async Task<DoctorDto> CreateAsync(CreateUpdateDoctorDto input)
        {
            var doctor = ObjectMapper.Map<EMRSystem.Doctors.Doctor>(input);
            doctor.TenantId = AbpSession.TenantId ?? input.TenantId;
            var doctorId = await Repository.InsertAndGetIdAsync(doctor);
            var doctorMaster = new DoctorMaster.DoctorMaster
            {
                TenantId = doctor.TenantId,
                DoctorId = doctorId,
                Fee = input.Fee
            };
            await _doctorMasterRepository.InsertAsync(doctorMaster);
            return MapToEntityDto(doctor);
        }
        public async Task<ListResultDto<DoctorDto>> GetAllDoctors()
        {
            var doctors = await Repository
                .GetAllIncluding(x => x.AbpUser, x => x.Department)
                .Where(x => !x.AbpUser.IsDeleted && !x.isEmergencyDoctor) // correct case
                .ToListAsync();

            var mapped = ObjectMapper.Map<List<DoctorDto>>(doctors);
            return new ListResultDto<DoctorDto>(mapped);
        }

        public async Task<ListResultDto<DoctorDto>> GetAllEmergencyDoctorsByTenantID(int tenantId)
        {
            var doctors = await Repository
                .GetAllIncluding(x => x.AbpUser, x => x.Department)
                .Where(x => x.TenantId == tenantId
                            && !x.AbpUser.IsDeleted
                            && x.isEmergencyDoctor)  // only emergency doctors
                .ToListAsync();

            var mapped = ObjectMapper.Map<List<DoctorDto>>(doctors);
            return new ListResultDto<DoctorDto>(mapped);
        }

        public EMRSystem.Doctors.Doctor GetDoctorDetailsByAbpUserID(long abpUserId)
        {
            var doctor = Repository.GetAll().FirstOrDefault(x => x.AbpUserId == abpUserId);

            if (doctor == null)
            {
                return null;
            }
            return doctor;
        }

        [HttpGet]
        public async Task<List<string>> GetCurrentUserRolesAsync()
        {
            if (!AbpSession.UserId.HasValue)
                return null;
            var user = await _userManager.GetUserByIdAsync(AbpSession.UserId.Value);
            var roles = await _userManager.GetRolesAsync(user);
            return roles.ToList();
        }
    }
}
