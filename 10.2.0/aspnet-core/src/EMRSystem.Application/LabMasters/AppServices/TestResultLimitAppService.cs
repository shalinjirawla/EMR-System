using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Domain.Entities;
using Abp.Domain.Repositories;
using Abp.EntityFrameworkCore;
using Abp.Extensions;
using EMRSystem.EntityFrameworkCore;
using EMRSystem.LabMasters.Dto.MeasureUnit;
using EMRSystem.LabMasters.Dto.TestResultLimit;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMRSystem.LabMasters.AppServices
{
    public class TestResultLimitAppService : AsyncCrudAppService<TestResultLimit, TestResultLimitDto, long, PagedTestResultLimitResultRequestDto, CreateUpdateTestResultLimitDto, CreateUpdateTestResultLimitDto>, ITestResultLimitAppService
    {
        private readonly IDbContextProvider<EMRSystemDbContext> _dbContextProvider;

        public TestResultLimitAppService(IDbContextProvider<EMRSystemDbContext> dbContextProvider, IRepository<TestResultLimit, long> repository) : base(repository)
        {
            _dbContextProvider = dbContextProvider;

        }
        protected override IQueryable<TestResultLimit> CreateFilteredQuery(PagedTestResultLimitResultRequestDto input)
        {
            var query = Repository.GetAll()
                .Include(x => x.LabTest) 
                .Where(x => x.TenantId == AbpSession.TenantId);

            if (!input.Keyword.IsNullOrWhiteSpace())
            {
                query = query.Where(x => x.LabTest.Name.Contains(input.Keyword));
            }

            return query;
        }
        public override async Task<TestResultLimitDto> GetAsync(EntityDto<long> input)
        {
            var entity = await Repository
                .GetAllIncluding(x => x.LabTest) // include LabTest for LabTest.Name
                .FirstOrDefaultAsync(x => x.Id == input.Id && x.TenantId == AbpSession.TenantId);

            if (entity == null)
            {
                throw new EntityNotFoundException(typeof(TestResultLimit), input.Id);
            }

            return ObjectMapper.Map<TestResultLimitDto>(entity);
        }


        public async Task<List<TestResultLimitDto>> CreateBulkAsync(List<CreateUpdateTestResultLimitDto> inputs)
        {
            var entities = ObjectMapper.Map<List<TestResultLimit>>(inputs);

            var dbContext = await _dbContextProvider.GetDbContextAsync();
            dbContext.TestResultLimits.AddRange(entities);  // ✅ fast batch insert
            await dbContext.SaveChangesAsync();

            return ObjectMapper.Map<List<TestResultLimitDto>>(entities);
        }
    }
}
