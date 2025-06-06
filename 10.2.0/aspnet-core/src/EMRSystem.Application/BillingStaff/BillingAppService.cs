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

namespace EMRSystem.BillingStaff
{
    [AbpAuthorize(PermissionNames.Pages_Billing)]
    public class BillingAppService : AsyncCrudAppService<Bill, BillingDto, long, PagedAndSortedResultRequestDto, CreateUpdateBillingDto, CreateUpdateBillingDto>,
   IBillingAppService
    {
        public BillingAppService(IRepository<Bill, long> repository) : base(repository)
        {
        }
    }

}
