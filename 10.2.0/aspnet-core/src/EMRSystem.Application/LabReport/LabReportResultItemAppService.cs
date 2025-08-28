using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Domain.Repositories;
using Abp.EntityFrameworkCore.Repositories;
using Abp.Timing;
using EMRSystem.Appointments;
using EMRSystem.Appointments.Dto;
using EMRSystem.IpdChargeEntry;
using EMRSystem.LabReport.Dto;
using EMRSystem.LabReports;
using EMRSystem.LabTechnician;
using EMRSystem.LabTechnician.Dto;
using EMRSystem.MultiTenancy;
using EMRSystem.Patients;
using EMRSystem.Prescriptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace EMRSystem.LabReport
{
    public class LabReportResultItemAppService : AsyncCrudAppService<EMRSystem.LabReports.LabReportResultItem, LabReportResultItemDto, long, PagedAndSortedResultRequestDto, CreateUpdateLabReportResultItemDto, CreateUpdateLabReportResultItemDto>,
  ILabReportResultItemAppService
    {
        private readonly IPrescriptionLabTestAppService _prescriptionLabTestsAppService;
        private readonly IRepository<EMRSystem.LabReports.PrescriptionLabTest, long> _prescriptionLabTestRepository;
        private readonly IRepository<Prescription, long> _prescriptionRepository;
        private readonly IRepository<Patient, long> _patientRepository;
        private readonly IRepository<EMRSystem.Admission.Admission, long> _admissionRepository;
        private readonly IRepository<EMRSystem.LabReportsTypes.LabReportsType, long> _labReportsTypeRepository;
        private readonly IRepository<EMRSystem.IpdChargeEntry.IpdChargeEntry, long> _ipdChargeEntryRepository;
        private readonly TenantManager _tenantManager;
        public LabReportResultItemAppService(IRepository<EMRSystem.LabReports.LabReportResultItem, long> repository
            , IPrescriptionLabTestAppService prescriptionLabTestsAppService,
             IRepository<EMRSystem.LabReports.PrescriptionLabTest, long> prescriptionLabTestRepository,
             IRepository<Prescription, long> prescriptionRepository,
             IRepository<Patient, long> patientRepository,
             IRepository<EMRSystem.Admission.Admission, long> admissionRepository,
             IRepository<EMRSystem.LabReportsTypes.LabReportsType, long> labReportsTypeRepository,
             IRepository<EMRSystem.IpdChargeEntry.IpdChargeEntry, long> ipdChargeEntryRepository,
            TenantManager tenantManager
            ) : base(repository)
        {
            _prescriptionLabTestsAppService = prescriptionLabTestsAppService;
            _tenantManager = tenantManager;
            _admissionRepository = admissionRepository;
            _prescriptionLabTestRepository = prescriptionLabTestRepository;
            _prescriptionRepository = prescriptionRepository;
            _patientRepository = patientRepository;
            _labReportsTypeRepository = labReportsTypeRepository;
            _ipdChargeEntryRepository = ipdChargeEntryRepository;
        }
        public async Task AddLabReportResultItem(List<CreateUpdateLabReportResultItemDto> reportResultItemDtos, [Optional] long? emergencyCaseId, bool isEmergencyCase = false)

        {
            try
            {
                if (reportResultItemDtos == null || reportResultItemDtos.Count == 0)
                    return;

                var mapped = ObjectMapper.Map<List<LabReportResultItem>>(reportResultItemDtos);
                await Repository.InsertRangeAsync(mapped);
                await CurrentUnitOfWork.SaveChangesAsync();

                var prescriptionLabTestId = reportResultItemDtos[0].PrescriptionLabTestId;
                await _prescriptionLabTestsAppService.MakeInprogressReports(prescriptionLabTestId);

                var prescriptionLabTest = await _prescriptionLabTestRepository.GetAsync(prescriptionLabTestId);

                var prescription = (prescriptionLabTest.PrescriptionId.HasValue) ? await _prescriptionRepository.GetAsync(prescriptionLabTest.PrescriptionId.Value) : null;

                var patient = (prescription != null && prescription.PatientId.HasValue) ? await _patientRepository.GetAsync(prescription.PatientId.Value) : null;

                var admission = (patient != null && patient.Id > 0) ? await _admissionRepository.FirstOrDefaultAsync(a => a.PatientId == patient.Id && a.IsDischarged == false) : null;

                var reportType = await _labReportsTypeRepository.GetAsync(prescriptionLabTest.LabReportsTypeId);

                if (!isEmergencyCase && admission != null && patient.IsAdmitted)
                {
                    var chargeEntry = new EMRSystem.IpdChargeEntry.IpdChargeEntry
                    {
                        TenantId = prescriptionLabTest.TenantId,
                        AdmissionId = admission.Id,
                        PatientId = patient.Id,
                        ChargeType = ChargeType.LabTest,
                        Description = $"Lab Test - {reportType?.ReportType ?? "Unknown"}",
                        Amount = reportType?.ReportPrice ?? 0,
                        EntryDate = Clock.Now,
                        IsProcessed = false,
                        ReferenceId = prescriptionLabTest.Id
                    };

                    await _ipdChargeEntryRepository.InsertAsync(chargeEntry);
                    await CurrentUnitOfWork.SaveChangesAsync();
                }
                else if (isEmergencyCase)
                {
                    var chargeEntry = new EMRSystem.EmergencyChargeEntries.EmergencyChargeEntry
                    {
                        PatientId = patient?.Id,
                        ChargeType = ChargeType.LabTest,
                        Description = $"Emergency Case Only",
                        Amount = reportType?.ReportPrice ?? 0,
                        EmergencyCaseId = emergencyCaseId,
                    };

                    await _emergencyChargeEntriesRepository.InsertAsync(chargeEntry);
                    await CurrentUnitOfWork.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                Logger.Error("Error in AddLabReportResultItem", ex);
                throw; // Consider throwing a user-friendly exception instead
            }
        }
            [HttpGet]
        public async Task<List<CreateUpdateLabReportResultItemDto>> GetLabReportResultItemsById(long prescriptionLabTestId)
        {
            var data = Repository.GetAll().Where(x => x.PrescriptionLabTestId == prescriptionLabTestId).ToList();
            var mapped = ObjectMapper.Map<List<CreateUpdateLabReportResultItemDto>>(data);
            return mapped;
        }
        public async Task EditLabReportResultItem(List<CreateUpdateLabReportResultItemDto> reportResultItemDtos)
        {
            try
            {
                if (reportResultItemDtos == null || reportResultItemDtos.Count == 0)
                    return;

                foreach (var dto in reportResultItemDtos)
                {
                    if (dto.Id > 0)
                    {
                        // Update existing record
                        var existingEntity = await Repository.GetAsync(dto.Id);
                        ObjectMapper.Map(dto, existingEntity); // This updates the properties
                        await Repository.UpdateAsync(existingEntity);
                    }
                    else
                    {
                        // Insert new record
                        var newEntity = ObjectMapper.Map<LabReportResultItem>(dto);
                        await Repository.InsertAsync(newEntity);
                    }
                }
                await CurrentUnitOfWork.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        [HttpGet]
        public async Task<ViewLabReportDto> ViewLabReport(long prescriptionLabTestId)
        {
            var data = _prescriptionLabTestsAppService.GetPrescriptionLabTestDetailsForViewReportById(prescriptionLabTestId);
            var newEntity = ObjectMapper.Map<ViewLabReportDto>(data);
            var tenant = await _tenantManager.GetByIdAsync(AbpSession.TenantId.Value);
            newEntity.TenantName = tenant?.Name;
            return newEntity;
        }
    }
}
