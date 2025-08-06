using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.UI;
using EMRSystem.Authorization;
using EMRSystem.Doctor.Dto;
using EMRSystem.LabMasters;
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
    private readonly IRepository<LabReportsType, long> _labReportTypeRepository;
    private readonly IRepository<LabReportTypeItem, long> _labReportTypeItemRepository;
    private readonly IRepository<LabTest, long> _labTestRepository;
    private readonly IRepository<TestResultLimit, long> _testResultLimitRepository;
        public LabReportsTypeAppService(IRepository<LabReportsType, long> repository,
            IRepository<LabReportsType, long> labReportTypeRepository,
            IRepository<LabReportTypeItem, long> labReportTypeItemRepository,
            IRepository<LabTest, long> labTestRepository,
            IRepository<TestResultLimit, long> testResultLimitRepository) : base(repository)
        {
            _labReportTypeRepository = labReportTypeRepository;
            _labReportTypeItemRepository = labReportTypeItemRepository;
            _labTestRepository = labTestRepository;
            _testResultLimitRepository = testResultLimitRepository;
        }

        public async Task<ListResultDto<LabReportsTypeDto>> GetAllTestByTenantID(int tenantId)
        {
            var query = Repository.GetAll()
                .Where(x => x.TenantId == tenantId); 
            var labReports = await query.ToListAsync();
            var mapped = ObjectMapper.Map<List<LabReportsTypeDto>>(labReports);
            return new ListResultDto<LabReportsTypeDto>(mapped);
        }

        public async Task<LabReportDetailDto> GetReportDetailsWithTestsAsync(long labReportTypeId)
        {
            var report = await _labReportTypeRepository.GetAsync(labReportTypeId);
            if (report == null)
            {
                throw new UserFriendlyException("Lab Report Type not found");
            }

            var reportItems = await _labReportTypeItemRepository
                .GetAll()
                .Where(x => x.LabReportTypeId == labReportTypeId)
                .ToListAsync();

            var labTestIds = reportItems.Select(x => x.LabTestId).Distinct().ToList();

            var labTests = await _labTestRepository
                .GetAllIncluding(x => x.MeasureUnit)
                .Where(x => labTestIds.Contains(x.Id))
                .ToListAsync();

            var resultLimits = await _testResultLimitRepository
                .GetAll()
                .Where(x => labTestIds.Contains(x.LabTestId))
                .ToListAsync();

            var labTestItems = labTests.Select(test =>
            {
                var limit = resultLimits.FirstOrDefault(r => r.LabTestId == test.Id);
                return new LabTestItemDto
                {
                    LabTestId = test.Id,
                    LabTestName = test.Name,
                    MeasureUnitName = test.MeasureUnit?.Name,
                    MinRange = limit?.MinRange,
                    MaxRange = limit?.MaxRange
                };
            }).ToList();

            return new LabReportDetailDto
            {
                ReportName = report.ReportType,
                ReportPrice = report.ReportPrice,
                LabTests = labTestItems
            };
        }

    }
}
