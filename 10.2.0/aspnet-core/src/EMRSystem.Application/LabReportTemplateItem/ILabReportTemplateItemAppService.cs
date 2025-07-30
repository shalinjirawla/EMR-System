using Abp.Application.Services;
using Abp.Application.Services.Dto;
using EMRSystem.LabReportTemplateItem.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMRSystem.LabReportTemplateItem
{
    public interface ILabReportTemplateItemAppService : IAsyncCrudAppService<
        LabReportTemplateItemDto, long, PagedAndSortedResultRequestDto,
        CreateUpdateLabReportTemplateItemDto, CreateUpdateLabReportTemplateItemDto>
    {
        Task<List<LabReportTemplateItemDto>> GetByLabReportsTypeIdAsync(long labReportsTypeId);
    }
}
