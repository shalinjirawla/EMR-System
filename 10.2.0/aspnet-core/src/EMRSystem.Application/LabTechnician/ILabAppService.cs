using Abp.Application.Services.Dto;
using Abp.Application.Services;
using EMRSystem.BillingStaff.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EMRSystem.LabReports;
using EMRSystem.LabTechnician.Dto;

namespace EMRSystem.LabTechnician
{
    public interface ILabAppService : IAsyncCrudAppService<
   LabReportDto, long, PagedAndSortedResultRequestDto, CreateUpdateLabReportDto, CreateUpdateLabReportDto>
    {
    }
}
