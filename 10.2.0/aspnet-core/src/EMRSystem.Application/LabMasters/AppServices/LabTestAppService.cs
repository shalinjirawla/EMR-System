using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Domain.Repositories;
using Abp.EntityFrameworkCore;
using Abp.Extensions;
using EMRSystem.EntityFrameworkCore;
using EMRSystem.LabMasters.Dto.LabReportTypeItem;
using EMRSystem.LabMasters.Dto.LabTest;
using EMRSystem.LabMasters.Dto.MeasureUnit;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Text;
using System.Threading.Tasks;

namespace EMRSystem.LabMasters.AppServices
{
    public class LabTestAppService : AsyncCrudAppService<
        LabTest,
        LabTestDto,
        long,
        PagedLabTestResultRequestDto,
        CreateUpdateLabTestDto,
        CreateUpdateLabTestDto>, ILabTestAppService
    {
        private readonly IDbContextProvider<EMRSystemDbContext> _dbContextProvider;

        public LabTestAppService(IDbContextProvider<EMRSystemDbContext> dbContextProvider, IRepository<LabTest, long> repository)
            : base(repository)
        {
            _dbContextProvider = dbContextProvider;
        }

        protected override IQueryable<LabTest> CreateFilteredQuery(PagedLabTestResultRequestDto input)
        {
            var query = Repository.GetAll()
                .Include(x => x.MeasureUnit)
                .Where(x => x.TenantId == AbpSession.TenantId);

            if (!input.Keyword.IsNullOrWhiteSpace())
            {
                query = query.Where(x => x.Name.Contains(input.Keyword));
            }

            if (input.IsActive.HasValue)
            {
                query = query.Where(x => x.IsActive == input.IsActive.Value);
            }
            query = !string.IsNullOrWhiteSpace(input.Sorting)
                    ? query.OrderBy(input.Sorting)
                    : query.OrderBy("Id desc");

            return query;
        }
        public async Task<List<LabTestDto>> CreateBulkAsync(List<CreateUpdateLabTestDto> inputs)
        {
            var entities = ObjectMapper.Map<List<LabTest>>(inputs);

            var dbContext = await _dbContextProvider.GetDbContextAsync();
            dbContext.LabTests.AddRange(entities); // 👈 Fast batch tracking
            await dbContext.SaveChangesAsync();        // 👈 One DB call only

            return ObjectMapper.Map<List<LabTestDto>>(entities);
        }
        public async Task<ListResultDto<LabTestDto>> GetAllLabTestByTenantIdAsync(int tenantId)
        {
            var allLabtests = await Repository.GetAllListAsync(x => x.TenantId == tenantId && x.IsActive);
            var mappedLabtests = ObjectMapper.Map<List<LabTestDto>>(allLabtests);
            return new ListResultDto<LabTestDto>(mappedLabtests);
        }
    }
}
