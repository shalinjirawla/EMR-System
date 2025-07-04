using Abp.Application.Services.Dto;
using Abp.Application.Services;
using EMRSystem.Visits.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EMRSystem.Departments.Dto;

namespace EMRSystem.Departments
{
    public interface IDepartmentAppService : IAsyncCrudAppService<
   DepartmentListDto, long, PagedAndSortedResultRequestDto, CreateUpdateDepartmentDto, CreateUpdateDepartmentDto>
    { }
}
