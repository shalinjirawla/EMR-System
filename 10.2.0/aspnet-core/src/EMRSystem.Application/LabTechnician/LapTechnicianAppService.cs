using Abp.Application.Services.Dto;
using Abp.Application.Services;
using Abp.Authorization;
using Abp.Domain.Repositories;
using EMRSystem.Authorization;
using EMRSystem.LabReports;
using EMRSystem.LabTechnician.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EMRSystem.Patients.Dto;
using Microsoft.AspNetCore.Mvc;
using EMRSystem.Authorization.Users;

namespace EMRSystem.LabTechnician
{
    public class LapTechnicianAppService :
        AsyncCrudAppService<EMRSystem.LabReports.LabTechnician, LabTechniciansDto, long, PagedAndSortedResultRequestDto, CreateUpdateLabTechnicianDto, CreateUpdateLabTechnicianDto>,
            ILapTechnicianAppService
    {
        private readonly UserManager _userManager;
        public LapTechnicianAppService(IRepository<EMRSystem.LabReports.LabTechnician, long> repository
            , UserManager userManager
            ) : base(repository)
        {
            _userManager = userManager;
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
