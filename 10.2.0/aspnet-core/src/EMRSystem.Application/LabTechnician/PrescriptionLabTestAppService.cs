using Abp.Application.Services.Dto;
using Abp.Application.Services;
using Abp.Domain.Repositories;
using EMRSystem.LabTechnician.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
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
            var query = (await Repository.GetAllAsync())
                        .Include(x => x.Prescription).ThenInclude(p => p.Patient)
                        .Include(x => x.Patient)                // ← add this
                        .Include(x => x.Prescription).ThenInclude(p => p.Doctor)
                        .Include(x => x.Prescription).ThenInclude(p => p.LabTests)
                        .Include(x => x.LabReportsType)
                        .WhereIf(AbpSession.TenantId.HasValue,
                                 x => x.TenantId == AbpSession.TenantId.Value);
            var totalCount = query.Count();

            var data = query.ToList();

            var mapped = ObjectMapper.Map<List<LabRequestListDto>>(data);
            return new PagedResultDto<LabRequestListDto>(totalCount, mapped);
        }
        [HttpGet]
        public async Task<PagedResultDto<LabOrderListDto>> GetAllLabOrders(PagedAndSortedResultRequestDto input)
        {
            var userId = AbpSession.UserId;
            var doctor = _doctorAppService.GetDoctorDetailsByAbpUserID(userId.Value);

            var dataa = await Repository.GetAllAsync();
            var query = dataa
                            .Include(x => x.Prescription).ThenInclude(x => x.Appointment)
                            .Include(x => x.Prescription).ThenInclude(x => x.Doctor)
                            .Include(x => x.Prescription).ThenInclude(x => x.Patient)
                            .Include(x => x.LabReportsType)
                            .WhereIf(AbpSession.TenantId.HasValue, x => x.TenantId == AbpSession.TenantId.Value)
                            .WhereIf(doctor != null, x => x.Prescription.Doctor.Id == doctor.Id)
                            .Where(x => x.TestStatus == LabTestStatus.Completed)

                            ;
            var totalCount = query.Count();
            var data = query.ToList();
            var mapped = ObjectMapper.Map<List<LabOrderListDto>>(data);
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
            // 1️⃣ Start from the repository, include everything you need
            var query = Repository.GetAll()
                .Include(x => x.LabReportResultItems)
                .Include(x => x.LabReportsType)
                .Include(x => x.Prescription).ThenInclude(p => p.Doctor)
                .Include(x => x.Prescription).ThenInclude(p => p.Patient)
                .Include(x => x.Patient)        // ← add this line

                // 2️⃣ Filter by the requested id *before* calling ToList/ToListAsync
                .Where(x => x.Id == id);

            // 3️⃣ Now fetch just that one entity
            var data = query.FirstOrDefault();
            return data; // will be null if not found
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
