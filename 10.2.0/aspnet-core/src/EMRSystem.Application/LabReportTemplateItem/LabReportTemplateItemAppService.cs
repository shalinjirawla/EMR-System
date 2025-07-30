using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Domain.Repositories;
using AutoMapper.Internal.Mappers;
using EMRSystem.LabReportTemplateItem.Dto;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMRSystem.LabReportTemplateItem
{
    public class LabReportTemplateItemAppService : AsyncCrudAppService<EMRSystem.LabReportTemplateItem.LabReportTemplateItem,LabReportTemplateItemDto,long,PagedAndSortedResultRequestDto,CreateUpdateLabReportTemplateItemDto,CreateUpdateLabReportTemplateItemDto>,
        ILabReportTemplateItemAppService
    {
        public LabReportTemplateItemAppService(
            IRepository<EMRSystem.LabReportTemplateItem.LabReportTemplateItem, long> repository
        ) : base(repository)
        {
        }

        public async Task<List<LabReportTemplateItemDto>> GetByLabReportsTypeIdAsync(long labReportsTypeId)
        {
            var query = await Repository
                .GetAllIncluding(x => x.LabReportsType)
                .Where(x => x.LabReportsTypeId == labReportsTypeId)
                .ToListAsync();

            return ObjectMapper.Map<List<LabReportTemplateItemDto>>(query);
        }
    }
}
