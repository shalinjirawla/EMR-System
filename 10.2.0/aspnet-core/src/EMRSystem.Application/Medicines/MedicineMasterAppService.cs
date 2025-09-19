using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Domain.Repositories;
using EMRSystem.Medicines.Dto;
using EMRSystem.Pharmacist.Dto;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMRSystem.Medicines
{
    public class MedicineMasterAppService :
        AsyncCrudAppService<MedicineMaster, MedicineMasterDto, long, PagedAndSortedResultRequestDto, CreateUpdateMedicineMasterDto>,
        IMedicineMasterAppService
    {
        public MedicineMasterAppService(IRepository<MedicineMaster, long> repository)
            : base(repository)
        {
        }
        protected override IQueryable<MedicineMaster> CreateFilteredQuery(PagedAndSortedResultRequestDto input)
        {
            return Repository.GetAll()
                .Include(x => x.Form)
                .Include(x => x.StrengthUnit);
        }

        // Map entity to DTO including names
        protected override MedicineMasterDto MapToEntityDto(MedicineMaster entity)
        {
            var dto = base.MapToEntityDto(entity);

            dto.FormName = entity.Form?.Name;
            dto.StrengthUnitName = entity.StrengthUnit?.Name;

            return dto;
        }
        public async Task<List<MedicineMasterDto>> GetAllListOfMedicine()
        {
            var list = await Repository.GetAll().Where(x => x.TenantId == AbpSession.TenantId).ToListAsync();
            var itemDtos = ObjectMapper.Map<List<MedicineMasterDto>>(list);
            return itemDtos;
        }

        public async Task<List<MedicineLookupDto>> GetMedicinesByFormIdAsync(long formId)
        {
            var tenantId = AbpSession.TenantId ?? 0;

            var query = Repository.GetAll()
                .Include(x => x.StrengthUnit) // strength unit fetch karna hai
                .Where(x => x.TenantId == tenantId && x.MedicineFormId == formId);

            var medicines = await query.ToListAsync();

            var result = medicines.Select(m => new MedicineLookupDto
            {
                Id = m.Id,
                MedicineName = m.Name,
                MedicineFormId = m.MedicineFormId,
                DosageOption = m.Strength.HasValue? $"{m.Strength.Value.ToString("0.##")} {m.StrengthUnit.Name}": $"N/A {m.StrengthUnit.Name}"

            }).ToList();


            return result;
        }

    }

}
