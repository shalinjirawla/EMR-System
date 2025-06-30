using Abp.Application.Services;
using Abp.Application.Services.Dto;
using EMRSystem.Appointments.Dto;
using EMRSystem.LabReport.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMRSystem.LabReport
{
    public interface ILabReportResultItemAppService : IAsyncCrudAppService<
   LabReportResultItemDto, long, PagedAndSortedResultRequestDto, CreateUpdateLabReportResultItemDto, CreateUpdateLabReportResultItemDto>
    {
    }
}
