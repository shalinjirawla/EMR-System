using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Domain.Repositories;
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
       PagedAndSortedResultRequestDto,
       CreateUpdateMeasureUnitDto,
       CreateUpdateMeasureUnitDto>, IMeasureUnitAppService
    {
        public MeasureUnitAppService(IRepository<MeasureUnit, long> repository)
            : base(repository)
        {
        }
        public async Task<List<MeasureUnitDto>> CreateBulkAsync(List<CreateUpdateMeasureUnitDto> inputs)
        {
            var created = new List<MeasureUnit>();

            foreach (var dto in inputs)
            {
                var entity = ObjectMapper.Map<MeasureUnit>(dto);
                await Repository.InsertAsync(entity);
                created.Add(entity);
            }

            await CurrentUnitOfWork.SaveChangesAsync();
            return ObjectMapper.Map<List<MeasureUnitDto>>(created);
        }
        public async Task<ListResultDto<MeasureUnitDto>> GetAllMeasureUnitsByTenantIdAsync(int tenantId)
        {
            var allUnits = await Repository.GetAllListAsync(x => x.TenantId == tenantId && x.IsActive);
            var mappedUnits = ObjectMapper.Map<List<MeasureUnitDto>>(allUnits);
            return new ListResultDto<MeasureUnitDto>(mappedUnits);
        }

    }
}
