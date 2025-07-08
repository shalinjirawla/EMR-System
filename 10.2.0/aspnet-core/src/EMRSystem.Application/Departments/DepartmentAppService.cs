
using Abp.Application.Services.Dto;
using Abp.Application.Services;
using Abp.Domain.Repositories;
using EMRSystem.Visits.Dto;
using EMRSystem.Visits;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EMRSystem.Departments.Dto;
using EMRSystem.LabReportsType.Dto;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;

namespace EMRSystem.Departments
{
    public class DepartmentAppService : AsyncCrudAppService<Department, DepartmentListDto, long, PagedAndSortedResultRequestDto, CreateUpdateDepartmentDto, CreateUpdateDepartmentDto>,
IDepartmentAppService
    {
        public DepartmentAppService(IRepository<Department, long> repository) : base(repository)
        {
        }
        [HttpGet]
        public async Task<ListResultDto<DepartmentListDto>> GetAllDepartmentByTenantID()
        {
            var query = Repository.GetAll()
                .Where(x => x.TenantId == AbpSession.TenantId.Value);
            var labReports = await query.ToListAsync();
            var mapped = ObjectMapper.Map<List<DepartmentListDto>>(labReports);
            return new ListResultDto<DepartmentListDto>(mapped);
        }
    }
}
