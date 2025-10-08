using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Domain.Repositories;
using Abp.EntityFrameworkCore;
using Abp.Extensions;
using EMRSystem.EntityFrameworkCore;
using EMRSystem.LabMasters.Dto.MeasureUnit;
using EMRSystem.MedicineForms.Dto;
using EMRSystem.StrengthUnitMaster.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMRSystem.StrengthUnitMaster
{
    public class StrengthUnitMasterAppService :
        AsyncCrudAppService<StrengthUnitMaster, StrengthUnitMasterDto, long, PagedStrengthUnitMasterDto, CreateUpdateStrengthUnitMasterDto, CreateUpdateStrengthUnitMasterDto>,
        IStrengthUnitMasterAppService
    {
        private readonly IDbContextProvider<EMRSystemDbContext> _dbContextProvider;

        public StrengthUnitMasterAppService(IDbContextProvider<EMRSystemDbContext> dbContextProvider, IRepository<StrengthUnitMaster, long> repository)
            : base(repository)
        {
            _dbContextProvider = dbContextProvider;
        }

        protected override IQueryable<StrengthUnitMaster> CreateFilteredQuery(PagedStrengthUnitMasterDto input)
        {
            var query = Repository.GetAll()
                .Where(x => x.TenantId == AbpSession.TenantId);

            if (!input.Keyword.IsNullOrWhiteSpace())
            {
                query = query.Where(x => x.Name.Contains(input.Keyword));
            }

            if (input.IsActive.HasValue)
            {
                query = query.Where(x => x.IsActive == input.IsActive.Value);
            }

            return query;
        }
        public async Task<List<StrengthUnitMasterDto>> CreateBulkAsync(List<CreateUpdateStrengthUnitMasterDto> inputs)
        {
            var entities = ObjectMapper.Map<List<StrengthUnitMaster>>(inputs);

            var dbContext = await _dbContextProvider.GetDbContextAsync();
            dbContext.StrengthUnitMasters.AddRange(entities); 
            await dbContext.SaveChangesAsync();      

            return ObjectMapper.Map<List<StrengthUnitMasterDto>>(entities);
        }
        public async Task<ListResultDto<StrengthUnitMasterDto>> GetAllStrengthUnitsByTenantIdAsync(int tenantId)
        {
            var allUnits = await Repository.GetAllListAsync(x => x.TenantId == tenantId && x.IsActive);
            var mappedUnits = ObjectMapper.Map<List<StrengthUnitMasterDto>>(allUnits);
            return new ListResultDto<StrengthUnitMasterDto>(mappedUnits);
        }
    }
}
