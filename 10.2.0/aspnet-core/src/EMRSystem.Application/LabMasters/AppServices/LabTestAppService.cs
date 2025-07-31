using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Domain.Repositories;
using Abp.Extensions;
using EMRSystem.LabMasters.Dto.LabTest;
using EMRSystem.LabMasters.Dto.MeasureUnit;
using System;
using System.Collections.Generic;
using System.Linq;
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
        public LabTestAppService(IRepository<LabTest, long> repository)
            : base(repository)
        {
        }

        protected override IQueryable<LabTest> CreateFilteredQuery(PagedLabTestResultRequestDto input)
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

        public async Task<List<LabTestDto>> CreateBulkAsync(List<CreateUpdateLabTestDto> inputs)
        {
            var created = new List<LabTest>();

            foreach (var dto in inputs)
            {
                var entity = ObjectMapper.Map<LabTest>(dto);
                await Repository.InsertAsync(entity);
                created.Add(entity);
            }

            await CurrentUnitOfWork.SaveChangesAsync();
            return ObjectMapper.Map<List<LabTestDto>>(created);
        }
    }
}
