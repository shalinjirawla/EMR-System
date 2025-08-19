using Abp.Application.Services.Dto;
using Abp.Application.Services;
using Abp.Domain.Repositories;
using EMRSystem.LabTechnician.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using Abp.Linq.Extensions;
using System.Linq.Dynamic.Core;
using System.Text;
using System.Threading.Tasks;
using EMRSystem.Patients.Dto;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Abp.Collections.Extensions;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using EMRSystem.Patients;
using Abp.EntityFrameworkCore.Extensions;
using EMRSystem.LabReport.Dto;
using EMRSystem.LabReports;
using EMRSystem.Doctor;
using EMRSystem.Authorization.Users;

namespace EMRSystem.LabTechnician
{
   
    public class PrescriptionLabTestAppService :
        AsyncCrudAppService<LabReports.PrescriptionLabTest, LabRequestListDto, long, PagedAndSortedResultRequestDto, CreateUpdateLabRequestDto, CreateUpdateLabRequestDto>,
            IPrescriptionLabTestAppService
    {
        private readonly IDoctorAppService _doctorAppService;
        private readonly UserManager _userManager;
        public PrescriptionLabTestAppService(IRepository<LabReports.PrescriptionLabTest, long> repository
            , IDoctorAppService doctorAppService, UserManager userManager
            ) : base(repository)
        {
            _doctorAppService = doctorAppService;
            _userManager = userManager;
        }
        [HttpGet]
        public async Task<PagedResultDto<LabRequestListDto>> GetAllLabTestRequests(PagedAndSortedResultRequestDto input)
        {
            // Repository.GetAll() returns IQueryable<TEntity> — important
            var query = Repository.GetAll()
                        .Include(x => x.Prescription).ThenInclude(p => p.Patient)
                        .Include(x => x.Patient)
                        .Include(x => x.Prescription).ThenInclude(p => p.Doctor)
                        .Include(x => x.Prescription).ThenInclude(p => p.LabTests)
                        .Include(x => x.LabReportsType)
                        .WhereIf(AbpSession.TenantId.HasValue, x => x.TenantId == AbpSession.TenantId.Value)
                        .Where(x => x.IsPaid == true);

            // total count before pagination
            var totalCount = await query.CountAsync();

            // Apply sorting (use input.Sorting if provided, else fallback)
            if (!string.IsNullOrWhiteSpace(input.Sorting))
            {
                // requires System.Linq.Dynamic.Core
                query = query.OrderBy(input.Sorting);
            }
            else
            {
                query = query.OrderByDescending(x => x.Id);
            }

            // Apply paging
            var items = await query
                .Skip(input.SkipCount)
                .Take(input.MaxResultCount)
                .ToListAsync();

            var mapped = ObjectMapper.Map<List<LabRequestListDto>>(items);

            return new PagedResultDto<LabRequestListDto>(totalCount, mapped);
        }
        [HttpGet]
        public async Task<PagedResultDto<LabOrderListDto>> GetAllLabOrders(PagedAndSortedResultRequestDto input)
        {
            var userId = AbpSession.UserId;

            // अगर _doctorAppService method async है तो await करो, वरना synchronous ही रहेगा
            var doctor = _doctorAppService.GetDoctorDetailsByAbpUserID(userId.Value);

            // base query without Includes for count
            var baseQuery = Repository.GetAll()
                .Where(x => x.Prescription != null)
                .WhereIf(AbpSession.TenantId.HasValue, x => x.TenantId == AbpSession.TenantId.Value)
                .WhereIf(doctor != null, x => x.Prescription.DoctorId == doctor.Id)
                .Where(x => x.TestStatus == LabTestStatus.Completed);

            // totalCount -> runs SELECT COUNT(*) ...
            var totalCount = await baseQuery.CountAsync();

            // now build the query for fetching page with includes
            var query = baseQuery
                .Include(x => x.Prescription).ThenInclude(p => p.Appointment)
                .Include(x => x.Prescription).ThenInclude(p => p.Doctor)
                .Include(x => x.Prescription).ThenInclude(p => p.Patient)
                .Include(x => x.LabReportsType)
                .AsNoTracking()
                .AsSplitQuery(); // optional: avoids cartesian explosion if collections exist

            // apply sorting (deterministic ordering important before Skip/Take)
            if (!string.IsNullOrWhiteSpace(input.Sorting))
                query = query.OrderBy(input.Sorting); // needs System.Linq.Dynamic.Core
            else
                query = query.OrderByDescending(x => x.Patient.FullName); // prefer CreationTime over bool

            // apply paging -> DB will receive OFFSET/FETCH
            var items = await query
                .Skip(input.SkipCount)
                .Take(input.MaxResultCount)
                .ToListAsync();

            var mapped = ObjectMapper.Map<List<LabOrderListDto>>(items);
            return new PagedResultDto<LabOrderListDto>(totalCount, mapped);
        }
        public EMRSystem.LabReports.PrescriptionLabTest GetPrescriptionLabTestDetailsById(long id)
        {
            var details = Repository.GetAll().ToList();
            if (details.Count > 0)
            {
                var data = details.FirstOrDefault(x => x.Id == id);
                return data;
            }
            return null;
        }
        public EMRSystem.LabReports.PrescriptionLabTest GetPrescriptionLabTestDetailsForViewReportById(long id)
        {
            var query = Repository.GetAll()
                .Include(x => x.LabReportResultItems)
                .Include(x => x.LabReportsType)
                .Include(x => x.Prescription).ThenInclude(p => p.Doctor)
                .Include(x => x.Prescription).ThenInclude(p => p.Patient)
                .Include(x => x.Patient)
                .Where(x => x.Id == id);
            var data = query.FirstOrDefault();
            return data;
        }
        public async Task MakeCompleteReport(long prescriptionLabTestId)
        {
            var data = GetPrescriptionLabTestDetailsById(prescriptionLabTestId);
            data.TestStatus = LabTestStatus.Completed;
            await Repository.UpdateAsync(data);
            CurrentUnitOfWork.SaveChanges();
        }
        public async Task MakeInprogressReports(long prescriptionLabTestId)
        {
            var data = GetPrescriptionLabTestDetailsById(prescriptionLabTestId);
            data.TestStatus = LabTestStatus.InProgress;
            await Repository.UpdateAsync(data);
            CurrentUnitOfWork.SaveChanges();
        }
        [HttpGet]
        public async Task<List<string>> GetCurrentUserRolesAsync()
        {
            if (!AbpSession.UserId.HasValue)
                return null;
            var user = await _userManager.GetUserByIdAsync(AbpSession.UserId.Value);
            var roles = await _userManager.GetRolesAsync(user);
            return roles.ToList();
        }
    }
}
