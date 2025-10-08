using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Domain.Repositories;
using Abp.EntityFrameworkCore;
using Abp.Extensions;
using EMRSystem.EntityFrameworkCore;
using EMRSystem.LabMasters;
using EMRSystem.LabMasters.Dto.MeasureUnit;
using EMRSystem.MedicineForms.Dto;
using EMRSystem.StrengthUnitMaster.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMRSystem.MedicineForms
{
    public class MedicineFormMasterAppService :
        AsyncCrudAppService<EMRSystem.MedicineFormMaster.MedicineFormMaster, MedicineFormMasterDto, long, PagedMedicineFormTypeDto, CreateUpdateMedicineFormMasterDto, CreateUpdateMedicineFormMasterDto>,
        IMedicineFormMasterAppService
    {
        private readonly IDbContextProvider<EMRSystemDbContext> _dbContextProvider;

        public MedicineFormMasterAppService(IDbContextProvider<EMRSystemDbContext> dbContextProvider,IRepository<EMRSystem.MedicineFormMaster.MedicineFormMaster, long> repository)
            : base(repository)
        {
            _dbContextProvider = dbContextProvider;
        }
        protected override IQueryable<EMRSystem.MedicineFormMaster.MedicineFormMaster> CreateFilteredQuery(PagedMedicineFormTypeDto input)
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
        public async Task<List<MedicineFormMasterDto>> CreateBulkAsync(List<CreateUpdateMedicineFormMasterDto> inputs)
        {
            var entities = ObjectMapper.Map<List<EMRSystem.MedicineFormMaster.MedicineFormMaster>>(inputs);

            var dbContext = await _dbContextProvider.GetDbContextAsync();
            dbContext.MedicineFormMasters.AddRange(entities); // 👈 Fast batch tracking
            await dbContext.SaveChangesAsync();        // 👈 One DB call only

            return ObjectMapper.Map<List<MedicineFormMasterDto>>(entities);
        }
        public async Task<ListResultDto<MedicineFormMasterDto>> GetAlldicineFormByTenantIdAsync(int tenantId)
        {
            var allUnits = await Repository.GetAllListAsync(x => x.TenantId == tenantId && x.IsActive);
            var mappedUnits = ObjectMapper.Map<List<MedicineFormMasterDto>>(allUnits);
            return new ListResultDto<MedicineFormMasterDto>(mappedUnits);
        }
    }
}
