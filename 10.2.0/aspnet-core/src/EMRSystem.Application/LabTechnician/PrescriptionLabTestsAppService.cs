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

namespace EMRSystem.LabTechnician
{
    public class PrescriptionLabTestsAppService :
        AsyncCrudAppService<LabReports.PrescriptionLabTest, LabRequestListDto, long, PagedAndSortedResultRequestDto, CreateUpdateLabRequestDto, CreateUpdateLabRequestDto>,
            IPrescriptionLabTestsAppService
    {
        private readonly IDoctorAppService _doctorAppService;
        public PrescriptionLabTestsAppService(IRepository<LabReports.PrescriptionLabTest, long> repository
            , IDoctorAppService doctorAppService
            ) : base(repository)
        {
            _doctorAppService = doctorAppService;
        }
        [HttpGet]
        public async Task<PagedResultDto<LabRequestListDto>> GetAllLabTestRequests(PagedAndSortedResultRequestDto input)
        {
            var dataa = await Repository.GetAllAsync();
            var query = dataa
                            .Include(x => x.Prescription).ThenInclude(x => x.Doctor)
                            .Include(x => x.Prescription).ThenInclude(x => x.Patient)
                            .Include(x => x.Prescription).ThenInclude(x => x.LabTests)
                            .Include(x => x.LabReportsType).WhereIf(AbpSession.TenantId.HasValue, x => x.TenantId == AbpSession.TenantId.Value);

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
            var details = Repository.GetAll()
                            .Include(x => x.LabReportResultItems)
                            .Include(x => x.LabReportsType)
                            .Include(x => x.Prescription).ThenInclude(x => x.Doctor)
                            .Include(x => x.Prescription).ThenInclude(x => x.Patient)
                            .ToList();
            if (details.Count > 0)
            {
                var data = details.FirstOrDefault(x => x.Id == id);
                return data;
            }
            return null;
        }
        public async Task MakeCompleteReport(long prescriptionLabTestId)
        {
            var data = GetPrescriptionLabTestDetailsById(prescriptionLabTestId);
            data.TestStatus = LabTestStatus.Completed;
            await Repository.UpdateAsync(data);
            CurrentUnitOfWork.SaveChanges();
        }
        public async Task MakeInprogressReport(long prescriptionLabTestId)
        {
            var data = GetPrescriptionLabTestDetailsById(prescriptionLabTestId);
            data.TestStatus = LabTestStatus.InProgress;
            await Repository.UpdateAsync(data);
            CurrentUnitOfWork.SaveChanges();
        }
    }
}
