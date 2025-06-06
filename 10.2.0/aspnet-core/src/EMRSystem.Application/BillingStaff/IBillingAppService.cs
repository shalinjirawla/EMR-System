using Abp.Application.Services.Dto;
using Abp.Application.Services;
using EMRSystem.Patients.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EMRSystem.BillingStaff.Dto;

namespace EMRSystem.BillingStaff
{
    public interface IBillingAppService : IAsyncCrudAppService<
    BillingDto, long, PagedAndSortedResultRequestDto, CreateUpdateBillingDto, CreateUpdateBillingDto>
    {
    }
}
