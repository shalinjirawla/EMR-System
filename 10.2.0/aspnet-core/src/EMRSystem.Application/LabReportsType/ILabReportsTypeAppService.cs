using Abp.Application.Services;
using Abp.Application.Services.Dto;
using EMRSystem.BillingStaff.Dto;
using EMRSystem.LabReportsType.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMRSystem.LabReportsType
{ 
    public interface ILabReportsTypeAppService : IAsyncCrudAppService<LabReportsTypeDto , long, PagedAndSortedResultRequestDto, CreateUpdateLabReportTypeDto, CreateUpdateLabReportTypeDto>
    {
    }
}
