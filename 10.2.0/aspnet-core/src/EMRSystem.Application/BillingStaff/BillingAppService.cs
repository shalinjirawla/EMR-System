using Abp.Application.Services.Dto;
using Abp.Application.Services;
using Abp.Domain.Repositories;
using EMRSystem.Patients.Dto;
using EMRSystem.Patients;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EMRSystem.Billings;
using EMRSystem.BillingStaff.Dto;
using Abp.Authorization;
using EMRSystem.Authorization;
using Microsoft.AspNetCore.Mvc;
using EMRSystem.Authorization.Users;

namespace EMRSystem.BillingStaff
{
    [AbpAuthorize(PermissionNames.Pages_Billing)]
    public class BillingAppService : AsyncCrudAppService<Bill, BillingDto, long, PagedAndSortedResultRequestDto, CreateUpdateBillingDto, CreateUpdateBillingDto>,
   IBillingAppService
    {
        private readonly UserManager _userManager;
        public BillingAppService(IRepository<Bill, long> repository
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
