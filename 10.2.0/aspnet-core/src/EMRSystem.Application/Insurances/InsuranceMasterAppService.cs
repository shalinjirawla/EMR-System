using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Domain.Repositories;
using Abp.Extensions;
using EMRSystem.Departments.Dto;
using EMRSystem.Insurances.Dto;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMRSystem.Insurances
{
    public class InsuranceMasterAppService : AsyncCrudAppService<InsuranceMaster, InsuranceMasterDto, long,
        PagedInsuranceMasterDto, CreateUpdateInsuranceMasterDto, CreateUpdateInsuranceMasterDto>,
        IInsuranceMasterAppService
    {
        public InsuranceMasterAppService(IRepository<InsuranceMaster, long> repository) : base(repository)
        {
        }

        protected override IQueryable<InsuranceMaster> CreateFilteredQuery(PagedInsuranceMasterDto input)
        {
            var query = Repository.GetAll()
               .Where(x => x.TenantId == AbpSession.TenantId);
            if (!input.Keyword.IsNullOrWhiteSpace())
            {
                query = query.Where(x => x.InsuranceName.Contains(input.Keyword));
            }

            if (input.IsActive.HasValue)
            {
                query = query.Where(x => x.IsActive == input.IsActive.Value);
            }

            return query;
        }
        public async Task<ListResultDto<InsuranceMasterDto>> GetAllInsuranceForDropdownAsync()
        {
            var insurance = await Repository
                     .GetAll()
                     .Where(x => x.IsActive && x.TenantId==AbpSession.TenantId)
                     .ToListAsync();

            return new ListResultDto<InsuranceMasterDto>(
                ObjectMapper.Map<List<InsuranceMasterDto>>(insurance)
            );
        }
    }
}
