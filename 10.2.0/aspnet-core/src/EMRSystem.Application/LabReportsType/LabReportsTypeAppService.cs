using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Domain.Repositories;
using EMRSystem.Authorization;
using EMRSystem.Doctor.Dto;
using EMRSystem.LabReportsType;
using EMRSystem.LabReportsType.Dto;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMRSystem.LabReportsTypes
{
    //[AbpAuthorize(PermissionNames.Pages_Users)]
    public class LabReportsTypeAppService : AsyncCrudAppService<LabReportsType, LabReportsTypeDto, long, PagedAndSortedResultRequestDto, CreateUpdateLabReportTypeDto, CreateUpdateLabReportTypeDto>,
  ILabReportsTypeAppService
    {
        public LabReportsTypeAppService(IRepository<LabReportsType, long> repository) : base(repository)
        {
        }

        public async Task<ListResultDto<LabReportsTypeDto>> GetAllTestByTenantID(int tenantId)
        {
            var query = Repository.GetAll()
                .Where(x => x.TenantId == tenantId); 
            var labReports = await query.ToListAsync();
            var mapped = ObjectMapper.Map<List<LabReportsTypeDto>>(labReports);
            return new ListResultDto<LabReportsTypeDto>(mapped);
        }
    }
}
