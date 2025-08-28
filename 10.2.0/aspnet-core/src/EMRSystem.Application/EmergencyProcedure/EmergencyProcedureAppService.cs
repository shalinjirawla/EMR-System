using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Domain.Repositories;
using Abp.EntityFrameworkCore;
using Abp.Extensions;
using Abp.Linq.Extensions;
using EMRSystem.Departments;
using EMRSystem.Departments.Dto;
using EMRSystem.EmergencyProcedure.Dto;
using EMRSystem.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMRSystem.EmergencyProcedure
{
    public class EmergencyProcedureAppService :
       AsyncCrudAppService<
           EmergencyProcedure,                 // Entity
           EmergencyProcedureDto,              // DTO
           long,                               // Primary key
           PagedEmergencyProcedureResultRequestDto,     // Paging/Sorting
           CreateUpdateEmergencyProcedureDto,  // Create DTO
           CreateUpdateEmergencyProcedureDto   // Update DTO
       >,
       IEmergencyProcedureAppService
    {
        private readonly IDbContextProvider<EMRSystemDbContext> _dbContextProvider;

        public EmergencyProcedureAppService(IDbContextProvider<EMRSystemDbContext> dbContextProvider, IRepository<EmergencyProcedure, long> repository)
            : base(repository)
        {
            _dbContextProvider = dbContextProvider;

        }
        protected override IQueryable<EmergencyProcedure> CreateFilteredQuery(PagedEmergencyProcedureResultRequestDto input)
        {
            var query = Repository
                .GetAll()
                .WhereIf(!input.Keyword.IsNullOrWhiteSpace(),
                    ep => ep.Name.Contains(input.Keyword))
                .WhereIf(input.Category.HasValue,
                    ep => ep.Category == input.Category.Value);

            return query;
        }

        public async Task<List<EmergencyProcedureDto>> CreateBulkAsync(List<CreateUpdateEmergencyProcedureDto> inputs)
        {
            var entities = ObjectMapper.Map<List<EmergencyProcedure>>(inputs);

            var dbContext = await _dbContextProvider.GetDbContextAsync();
            dbContext.EmergencyProcedures.AddRange(entities); // 👈 Fast batch tracking
            await dbContext.SaveChangesAsync();        // 👈 One DB call only

            return ObjectMapper.Map<List<EmergencyProcedureDto>>(entities);
        }
    }
}
