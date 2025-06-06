using Abp.Application.Services.Dto;
using Abp.Application.Services;
using Abp.Domain.Repositories;
using EMRSystem.Billings;
using EMRSystem.BillingStaff.Dto;
using EMRSystem.LabReports;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EMRSystem.LabTechnician.Dto;
using Abp.Authorization;
using EMRSystem.Authorization;

namespace EMRSystem.LabTechnician
{
    [AbpAuthorize(PermissionNames.Pages_LabReports)]
    public class LabAppService : AsyncCrudAppService<LabReport, LabDto, long, PagedAndSortedResultRequestDto, CreateUpdateLabDto, CreateUpdateLabDto>,
   ILabAppService
    {
        public LabAppService(IRepository<LabReport, long> repository) : base(repository)
        {
        }
    }
}
