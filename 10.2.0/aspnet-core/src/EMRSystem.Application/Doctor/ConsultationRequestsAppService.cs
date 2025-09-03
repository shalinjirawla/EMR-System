using Abp.Application.Services.Dto;
using Abp.Application.Services;
using EMRSystem.Doctor.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using Microsoft.AspNetCore.Mvc;
using EMRSystem.Authorization.Users;
using EMRSystem.Admissions.Dto;
using Microsoft.EntityFrameworkCore;
using Abp.Collections.Extensions;
using EMRSystem.Doctors;

namespace EMRSystem.Doctor
{
    public class ConsultationRequestsAppService : AsyncCrudAppService<EMRSystem.Doctors.ConsultationRequests, ConsultationRequestsDto, long, PagedAndSortedResultRequestDto, CreateUpdateConsultationRequestsDto, CreateUpdateConsultationRequestsDto>,
IConsultationRequestsAppService
    {
        private readonly UserManager _userManager;
        public ConsultationRequestsAppService(
            IRepository<EMRSystem.Doctors.ConsultationRequests, long> Repository,
                UserManager userManager
            ) : base(Repository)
        {
            _userManager = userManager;
        }
        protected override IQueryable<EMRSystem.Doctors.ConsultationRequests> CreateFilteredQuery(PagedAndSortedResultRequestDto input)
        {
            var res = Repository.GetAll()
                .Include(x => x.RequestedSpecialist)
                .Include(x => x.RequestingDoctor)
                .Include(x => x.Prescriptions).ThenInclude(x => x.Patient);
            return res;
        }
        [HttpGet]
        public async Task<List<string>> GetCurrentUserRolesAsync()
        {
            var user = await _userManager.GetUserByIdAsync(AbpSession.UserId.Value);
            var roles = await _userManager.GetRolesAsync(user);
            return roles.ToList();
        }
    }
}
