using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Domain.Repositories;
using Abp.EntityFrameworkCore;
using Abp.Extensions;
using EMRSystem.EntityFrameworkCore;
using EMRSystem.LabMasters.Dto.LabTest;
using EMRSystem.LabMasters.Dto.MeasureUnit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMRSystem.LabMasters.AppServices
{
    public class MeasureUnitAppService : AsyncCrudAppService<
       MeasureUnit,
       MeasureUnitDto,
       long,
       PagedMeasureUnitResultRequestDto,
       CreateUpdateMeasureUnitDto,
       CreateUpdateMeasureUnitDto>, IMeasureUnitAppService
    {
        private readonly IDbContextProvider<EMRSystemDbContext> _dbContextProvider;
        public MeasureUnitAppService(IDbContextProvider<EMRSystemDbContext> dbContextProvider,IRepository<MeasureUnit, long> repository)
            : base(repository)
        {
            _dbContextProvider = dbContextProvider;
        }
        //public async Task<List<MeasureUnitDto>> CreateBulkAsync(List<CreateUpdateMeasureUnitDto> inputs)
        //{
        //    var created = new List<MeasureUnit>();

        //    foreach (var dto in inputs)
        //    {
        //        var entity = ObjectMapper.Map<MeasureUnit>(dto);
        //        await Repository.InsertAsync(entity);
        //        created.Add(entity);
        //    }

        //    await CurrentUnitOfWork.SaveChangesAsync();
        //    return ObjectMapper.Map<List<MeasureUnitDto>>(created);
        //}
        public async Task<List<MeasureUnitDto>> CreateBulkAsync(List<CreateUpdateMeasureUnitDto> inputs)
        {
            var entities = ObjectMapper.Map<List<MeasureUnit>>(inputs);

            var dbContext = await _dbContextProvider.GetDbContextAsync();
            dbContext.MeasureUnits.AddRange(entities); // 👈 Fast batch tracking
            await dbContext.SaveChangesAsync();        // 👈 One DB call only

            return ObjectMapper.Map<List<MeasureUnitDto>>(entities);
        }
        public async Task<ListResultDto<MeasureUnitDto>> GetAllMeasureUnitsByTenantIdAsync(int tenantId)
        {
            var allUnits = await Repository.GetAllListAsync(x => x.TenantId == tenantId && x.IsActive);
            var mappedUnits = ObjectMapper.Map<List<MeasureUnitDto>>(allUnits);
            return new ListResultDto<MeasureUnitDto>(mappedUnits);
        }
        protected override IQueryable<MeasureUnit> CreateFilteredQuery(PagedMeasureUnitResultRequestDto input)
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

    }
}
