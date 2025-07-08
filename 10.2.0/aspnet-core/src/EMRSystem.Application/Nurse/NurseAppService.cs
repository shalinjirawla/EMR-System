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
using Abp.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using EMRSystem.Authorization.Users;

namespace EMRSystem.Nurse
{
    //[AbpAuthorize(PermissionNames.Pages_Nurses)]
    public class NurseAppService : AsyncCrudAppService<EMRSystem.Nurses.Nurse, NurseDto, long, PagedAndSortedResultRequestDto, CreateUpdateNurseDto, CreateUpdateNurseDto>,
   INurseAppService
    {
        private readonly UserManager _userManager;
        public NurseAppService(IRepository<EMRSystem.Nurses.Nurse, long> repository
            , UserManager userManager
            ) : base(repository)
        {
            _userManager = userManager;
        }

        public async Task<ListResultDto<NurseDto>> GetAllNursesByTenantID(int tenantId)
        {
            var doctorDto = await Repository.GetAllIncludingAsync(x => x.AbpUser);
            var list = doctorDto.Where(x => x.TenantId == tenantId && !x.AbpUser.IsDeleted);
            var mapped = ObjectMapper.Map<List<NurseDto>>(list);
            var resultList = new ListResultDto<NurseDto>(mapped);
            return resultList;
        }
        public EMRSystem.Nurses.Nurse GetNurseDetailsByAbpUserID(long abpUserId)
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
