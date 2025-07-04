using Abp.Application.Services;
using Abp.Application.Services.Dto;
using EMRSystem.Patients.Dto;
using EMRSystem.Visits.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMRSystem.Visits
{
    public interface IVisitAppService : IAsyncCrudAppService<
   VisitListDto, long, PagedAndSortedResultRequestDto, CreateUpdateVisitDto, CreateUpdateVisitDto>
    { }
}
