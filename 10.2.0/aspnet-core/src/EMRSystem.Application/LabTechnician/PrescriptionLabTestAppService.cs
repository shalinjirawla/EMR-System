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
                        .Include(x => x.Patient)
                        .Include(x => x.Prescription).ThenInclude(p => p.Doctor)
                        .Include(x => x.Prescription).ThenInclude(p => p.LabTests)
                        .Include(x => x.LabReportsType)
                        .WhereIf(AbpSession.TenantId.HasValue,
                                 x => x.TenantId == AbpSession.TenantId.Value)
                        .Where(x => x.IsPaid == true);

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

            // Build IQueryable<PrescriptionLabTest>
            var query = Repository.GetAll()
                .Include(x => x.Prescription).ThenInclude(x => x.Appointment)
                .Include(x => x.Prescription).ThenInclude(x => x.Doctor)
                .Include(x => x.Prescription).ThenInclude(x => x.Patient)
                .Include(x => x.LabReportsType)
                .Where(x => x.Prescription != null)
                .WhereIf(AbpSession.TenantId.HasValue,
                         x => x.TenantId == AbpSession.TenantId.Value)
                .WhereIf(doctor != null,
                         x => x.Prescription.DoctorId == doctor.Id)
                .Where(x => x.TestStatus == LabTestStatus.Completed);

            // Execute counts and list asynchronously
            var totalCount =  query.Count();
            var data =  query.ToList();

            // Map and return
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
