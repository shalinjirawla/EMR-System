using Abp.Application.Services;
using Abp.Domain.Repositories;
using Abp.Extensions;
using Abp.Linq.Extensions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMRSystem.Emergency.Triage
{
    public class TriageAppService :
     AsyncCrudAppService<Triage, TriageDto, long, PagedTriageResultRequestDto, CreateUpdateTriageDto>,
     ITriageAppService
    {
        public TriageAppService(IRepository<Triage, long> repository)
            : base(repository)
        {
        }

        protected override IQueryable<Triage> CreateFilteredQuery(PagedTriageResultRequestDto input)
        {
            return Repository.GetAll()
                .Include(x => x.EmergencyCase)
                .WhereIf(!input.Keyword.IsNullOrWhiteSpace(),x => x.EmergencyCase.EmergencyNumber.Contains(input.Keyword));
        }

    }

}
