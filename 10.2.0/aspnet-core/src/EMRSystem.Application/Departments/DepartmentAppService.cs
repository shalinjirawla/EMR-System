
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Domain.Repositories;
using Abp.EntityFrameworkCore;
using EMRSystem.Departments.Dto;
using EMRSystem.EntityFrameworkCore;
using EMRSystem.LabMasters;
using EMRSystem.LabMasters.Dto.MeasureUnit;
using EMRSystem.LabReportsType.Dto;
using EMRSystem.Visits;
using EMRSystem.Visits.Dto;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMRSystem.Departments
{
    public class DepartmentAppService : AsyncCrudAppService<
            Department,
            DepartmentDto,
            long,
            PagedAndSortedResultRequestDto,
            CreateUpdateDepartmentDto,
            CreateUpdateDepartmentDto>,
        IDepartmentAppService
    {
        private readonly IDbContextProvider<EMRSystemDbContext> _dbContextProvider;

        public DepartmentAppService(IDbContextProvider<EMRSystemDbContext> dbContextProvider, IRepository<Department, long> repository)
            : base(repository)
        {
            _dbContextProvider = dbContextProvider;
        }

        public async Task<List<DepartmentDto>> CreateBulkAsync(List<CreateUpdateDepartmentDto> inputs)
        {
            var entities = ObjectMapper.Map<List<Department>>(inputs);

            var dbContext = await _dbContextProvider.GetDbContextAsync();
            dbContext.Departments.AddRange(entities); // 👈 Fast batch tracking
            await dbContext.SaveChangesAsync();        // 👈 One DB call only

            return ObjectMapper.Map<List<DepartmentDto>>(entities);
        }
        public async Task<ListResultDto<DepartmentDto>> GetAllDepartmentForDropdownAsync()
        {
            var departments = await Repository
         .GetAll()
         .Where(x => x.IsActive)
         .ToListAsync();

            return new ListResultDto<DepartmentDto>(
                ObjectMapper.Map<List<DepartmentDto>>(departments)
            );
        }
        public async Task<ListResultDto<DepartmentDto>> GetAllDepartmentForDoctorAsync()
        {
            var departments = await Repository
                .GetAll()
                .Where(x => x.DepartmentType == DepartmentType.Doctor && x.IsActive)
                .ToListAsync();

            return new ListResultDto<DepartmentDto>(
                ObjectMapper.Map<List<DepartmentDto>>(departments)
            );
        }

        public async Task<ListResultDto<DepartmentDto>> GetAllDepartmentForLabTechnicianAsync()
        {
            var departments = await Repository
                .GetAll()
                .Where(x => x.DepartmentType == DepartmentType.LabTechnician && x.IsActive)
                .ToListAsync();

            return new ListResultDto<DepartmentDto>(
                ObjectMapper.Map<List<DepartmentDto>>(departments)
            );
        }


    }
}