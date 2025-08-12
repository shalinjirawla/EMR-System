using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Domain.Repositories;
using Abp.EntityFrameworkCore;
using EMRSystem.EntityFrameworkCore;
using EMRSystem.LabMasters.Dto.LabReportTypeItem;
using EMRSystem.LabMasters.Dto.LabTest;
using EMRSystem.LabMasters.Dto.MeasureUnit;
using EMRSystem.LabReportsType.Dto;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMRSystem.LabMasters.AppServices
{
    public class LabReportTypeItemAppService : AsyncCrudAppService<
        LabReportTypeItem,
        LabReportTypeItemDto,
        long,
        PagedAndSortedResultRequestDto,
        CreateUpdateLabReportTypeItemDto,
        CreateUpdateLabReportTypeItemDto>, ILabReportTypeItemAppService
    {
        private readonly IDbContextProvider<EMRSystemDbContext> _dbContextProvider;
        private readonly IRepository<LabTest, long> _labTestRepo;
        public LabReportTypeItemAppService(IDbContextProvider<EMRSystemDbContext> dbContextProvider, 
            IRepository<LabReportTypeItem, long> repository,
            IRepository<LabTest, long> labTestRepo)
            : base(repository)
        {
            _dbContextProvider = dbContextProvider;
            _labTestRepo = labTestRepo;
        }

        protected override IQueryable<LabReportTypeItem> CreateFilteredQuery(PagedAndSortedResultRequestDto input)
        {
            return Repository
                .GetAllIncluding(x => x.LabTest)
                .Where(x => x.IsActive);
        }
        public async Task<ListResultDto<LabReportsTypeDto>> GetAllLabReportTypeByTenantIdAsync(int tenantId)
        {
            var allReporttype = await Repository.GetAllListAsync(x => x.TenantId == tenantId && x.IsActive);
            var mappedReportType = ObjectMapper.Map<List<LabReportsTypeDto>>(allReporttype);
            return new ListResultDto<LabReportsTypeDto>(mappedReportType);
        }
        //public async Task<ListResultDto<LabReportTypeItemDto>> GetAllLabReportItems(int id)
        //{
        //    var allReporttypeitem = await Repository.GetAllListAsync(x => x.LabReportTypeId == id && x.IsActive);
        //    var mappedReportType = ObjectMapper.Map<List<LabReportTypeItemDto>>(allReporttypeitem);
        //    return new ListResultDto<LabReportTypeItemDto>(mappedReportType);
        //}
        public async Task<ListResultDto<LabReportTestWithUnitDto>> GetAllLabReportItemsAsync(int reportTypeId)
        {
            var query = Repository.GetAll()
                .Where(x => x.LabReportTypeId == reportTypeId)
                .Include(x => x.LabTest)
                    .ThenInclude(t => t.MeasureUnit);

            var list = await query.ToListAsync();

            var dtos = list.Select(x => new LabReportTestWithUnitDto
            {
                Id = x.Id,
                LabReportTypeId = x.LabReportTypeId,
                LabTestId = x.LabTestId,
                LabTestName = x.LabTest.Name,
                MeasureUnitId = x.LabTest.MeasureUnitId,
                MeasureUnitName = x.LabTest.MeasureUnit?.Name,
                IsActive = x.LabTest.IsActive
            }).ToList();

            // Wrap your List<T> in a ListResultDto<T>:
            return new ListResultDto<LabReportTestWithUnitDto>(dtos);
        }

        public async Task<List<LabReportTypeItemDto>> CreateBulkAsync(List<CreateUpdateLabReportTypeItemDto> inputs)
        {
            var entities = ObjectMapper.Map<List<LabReportTypeItem>>(inputs);

            var dbContext = await _dbContextProvider.GetDbContextAsync();
            dbContext.LabReportTypeItems.AddRange(entities); // 👈 Fast batch tracking
            await dbContext.SaveChangesAsync();        // 👈 One DB call only

            return ObjectMapper.Map<List<LabReportTypeItemDto>>(entities);
        }
    }
}
