using Abp.Application.Services.Dto;
using Abp.Application.Services;
using EMRSystem.Nurse.Dto;
using EMRSystem.Nurse;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EMRSystem.Patient_Discharge.Dto;
using Abp.Domain.Repositories;
using Abp.Domain.Entities;
using Abp.UI;
using Microsoft.AspNetCore.Mvc;
using EMRSystem.Authorization.Users;
using EMRSystem.Users.Dto;
using Abp.Collections.Extensions;
using Microsoft.EntityFrameworkCore;
using EMRSystem.PatientDischarge;
using System.Security.Cryptography.X509Certificates;
using Abp.Authorization.Users;
using EMRSystem.Patients;
using static Castle.MicroKernel.ModelBuilder.Descriptors.InterceptorDescriptor;
using EMRSystem.Appointments;
using EMRSystem.Prescriptions;
using Abp.Domain.Uow;
using EMRSystem.Vitals.Dto;
using EMRSystem.Prescriptions.Dto;
using EMRSystem.LabTechnician;
using EMRSystem.PrescriptionLabTest;
using EMRSystem.EmergencyProcedure;
using EMRSystem.Vitals;
using EMRSystem.Invoice;
using EMRSystem.Doctors;
using Abp.Linq.Extensions;
using Abp.Extensions;
using EMRSystem.Pharmacist;
using EMRSystem.RoomMaster;

namespace EMRSystem.Patient_Discharge
{
    public class PatientDischargeAppService : AsyncCrudAppService<EMRSystem.PatientDischarge.PatientDischarge, PatientDischargeDto, long, PagedPatientDischargeResultRequestDto, CreateUpdatePatientDischargeDto, CreateUpdatePatientDischargeDto>,
   IPatientDischargeAppService
    {
        private readonly IPharmacistPrescriptionsAppService _prescriptionAppService;
        private readonly ICreatePrescriptionLabTestsAppService _createPrescriptionLabTestAppService;
        private readonly ISelectedEmergencyProceduresAppService _selectedEmergencyProceduresAppService;
        private readonly IVitalAppService _vitalAppService;
        private readonly IInvoiceAppService _invoiceAppService;
        private readonly IRepository<EMRSystem.Admission.Admission, long> _admissionAppService;
        private readonly IRepository<EMRSystem.RoomMaster.Bed, long> _bedAppService;
        private readonly IRepository<EMRSystem.Patients.Patient, long> _patientService;
        private readonly IRepository<EMRSystem.Emergency.EmergencyCase.EmergencyCase, long> _emergencyCaseAppService;
        public PatientDischargeAppService(IRepository<EMRSystem.PatientDischarge.PatientDischarge, long> repository,
            IPharmacistPrescriptionsAppService prescriptionAppService, ICreatePrescriptionLabTestsAppService createPrescriptionLabTestAppService,
            ISelectedEmergencyProceduresAppService selectedEmergencyProceduresAppService,
            IVitalAppService vitalAppService, IInvoiceAppService invoiceAppService,
            IRepository<EMRSystem.Admission.Admission, long> admissionAppService,
            IRepository<EMRSystem.RoomMaster.Bed, long> bedAppService,
            IRepository<EMRSystem.Patients.Patient, long> patientService,
        IRepository<EMRSystem.Emergency.EmergencyCase.EmergencyCase, long> emergencyCaseAppService
            ) : base(repository)
        {
            _prescriptionAppService = prescriptionAppService;
            _createPrescriptionLabTestAppService = createPrescriptionLabTestAppService;
            _selectedEmergencyProceduresAppService = selectedEmergencyProceduresAppService;
            _vitalAppService = vitalAppService;
            _invoiceAppService = invoiceAppService;
            _admissionAppService = admissionAppService;
            _emergencyCaseAppService = emergencyCaseAppService;
            _bedAppService = bedAppService;
            _patientService = patientService;
        }

        protected override IQueryable<EMRSystem.PatientDischarge.PatientDischarge> CreateFilteredQuery(PagedPatientDischargeResultRequestDto input)
        {
            var data = Repository.GetAllIncluding(
                x => x.Admission,
                x => x.Admission.Room,
                x => x.Admission.Room.RoomTypeMaster,
                x => x.Admission.Bed,
                x => x.Patient,
                x => x.Doctor)
                .WhereIf(!input.Keyword.IsNullOrWhiteSpace(), x => x.Patient.FullName.Contains(input.Keyword))
                .Where(x => x.Admission.IsDischarged == true);
            return data;
        }
        public async Task DischargeStatusChange(long patientID, DischargeStatus status)
        {
            try
            {
                var discharge = await Repository.GetAllIncluding(x => x.Patient).FirstOrDefaultAsync(x => x.PatientId == patientID);
                if (discharge.DischargeStatus != status)
                {
                    discharge.DischargeStatus = status;
                    await Repository.UpdateAsync(discharge);
                }
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException("No Patient Data found");
            }
        }

        [HttpGet]
        public async Task<DischargeSummaryDto> PatientDischargeSummaryAsync(long patientID)
        {
            var data = await Repository
            .GetAll().IgnoreQueryFilters()
            .Include(x => x.Patient).ThenInclude(x => x.AbpUser)
            .FirstOrDefaultAsync(x => x.PatientId == patientID);

            var vitalsList = await _vitalAppService.GetVitalsByPatientID(patientID);
            var precriptionList = await _prescriptionAppService.GetPharmacistPrescriptionsByPatient(patientID);
            var precriptionLabTestList = await _createPrescriptionLabTestAppService.GetPrescriptionLabTestByPatientId(patientID);
            var selectedEmergencyProcedures = await _selectedEmergencyProceduresAppService.GetSelectedProceduresByPatientID(patientID);
            var invoiceList = await _invoiceAppService.GetInvoicesByPatientID(patientID);
            var res = ObjectMapper.Map<DischargeSummaryDto>(data);

            if (vitalsList != null && vitalsList.Count > 0)
            {
                res.Vitals = vitalsList;
            }
            if (precriptionList.Count > 0)
            {
                res.Prescriptions = precriptionList.ToList();
            }
            if (precriptionLabTestList != null && precriptionLabTestList.Count > 0)
            {
                res.PrescriptionLabTests = precriptionLabTestList;
            }
            if (selectedEmergencyProcedures != null && selectedEmergencyProcedures.Count > 0)
            {
                res.SelectedEmergencyProcedures = selectedEmergencyProcedures;
            }
            if (invoiceList != null && invoiceList.Count > 0)
            {
                res.Invoices = invoiceList;
            }
            return res;

        }

        [HttpPost]
        public async Task FinalApproval(string summary, long patientID, long doctorID)
        {
            var discharge = await Repository.GetAll().FirstOrDefaultAsync(x => x.PatientId == patientID);
            if (discharge != null)
            {
                discharge.DischargeStatus = DischargeStatus.FinalApproval;
                discharge.DoctorId = doctorID;
                discharge.DischargeSummary = summary?.Trim();
                await Repository.UpdateAsync(discharge);
            }
        }

        [HttpPost]
        public async Task FinalDischarge(long patientID)
        {
            var discharge = await Repository.GetAll().FirstOrDefaultAsync(x => x.PatientId == patientID);
            if (discharge != null)
            {
                discharge.DischargeStatus = DischargeStatus.Discharged;
                discharge.DischargeDate = DateTime.Now;
                await Repository.UpdateAsync(discharge);
                var getAdmission = await _admissionAppService.GetAllIncluding(x => x.Bed).Where(x => x.PatientId == patientID).ToListAsync();
                if (getAdmission.Count > 0)
                {
                    getAdmission.ForEach(async x =>
                    {
                        x.IsDischarged = true;
                        x.DischargeDateTime = discharge.DischargeDate;
                        await _admissionAppService.UpdateAsync(x);
                        var getBedDetails = await _bedAppService.GetAsync(x.BedId.Value);
                        if (getBedDetails != null)
                        {
                            getBedDetails.Status = BedStatus.Available;
                            await _bedAppService.UpdateAsync(getBedDetails);
                        }
                    });
                }
                var emergencyCase = await _emergencyCaseAppService.GetAll().Where(x => x.PatientId == patientID).ToListAsync();
                if (emergencyCase.Count > 0)
                {
                    emergencyCase.ForEach(async x =>
                    {
                        x.Status = Emergency.EmergencyCase.EmergencyStatus.Discharged;
                        x.DischargeTime = discharge.DischargeDate;
                        await _emergencyCaseAppService.UpdateAsync(x);
                    });
                }
                var patient = await _patientService.GetAsync(patientID);
                if (patient != null)
                {
                    patient.IsAdmitted = false;
                    await _patientService.UpdateAsync(patient);
                }
            }
        }

        public async Task DownloadPDF()
        {

        }
    }
}
