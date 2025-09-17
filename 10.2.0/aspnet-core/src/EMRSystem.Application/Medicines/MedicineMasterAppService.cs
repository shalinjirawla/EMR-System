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
    }

}
