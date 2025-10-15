using Abp.Application.Services;
using Abp.Domain.Repositories;
using Abp.Extensions;
using Abp.Linq.Extensions;
using EMRSystem.Appointments.Dto;
using EMRSystem.Appointments;
using EMRSystem.Emergency.EmergencyCase;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using Abp.Domain.Entities;
using Abp.UI;
using EMRSystem.Doctors;
using EMRSystem.IpdChargeEntry;
using EMRSystem.Patients;
using EMRSystem.Admission;
using Stripe.V2;

namespace EMRSystem.Emergency.EmergencyCase
{
    public class EmergencyAppService :
         AsyncCrudAppService<EmergencyCase, EmergencyCaseDto, long, PagedEmergencyCaseResultRequestDto, CreateUpdateEmergencyCaseDto, CreateUpdateEmergencyCaseDto>,
         IEmergencyAppService
    {
        private readonly IRepository<Patient, long> _patientRepository;
        private readonly IRepository<EMRSystem.Emergency.EmergencyMaster.EmergencyMaster, long> _emergencyMasterRepository;
        private readonly IRepository<EMRSystem.IpdChargeEntry.IpdChargeEntry, long> _ipdChargeEntryRepository;
        private readonly IRepository<EMRSystem.EmergencyChargeEntries.EmergencyChargeEntry, long> _emergencyChargeEntriesRepository;
        private readonly IRepository<EMRSystem.Prescriptions.Prescription, long> _prescriptionRepository;
        private readonly IRepository<EMRSystem.LabReports.PrescriptionLabTest, long> _prescriptionLabTestRepository;
        public EmergencyAppService(IRepository<EmergencyCase, long> repository, IRepository<Patient, long> patientRepository,
                IRepository<EMRSystem.IpdChargeEntry.IpdChargeEntry, long> ipdChargeEntryRepository, IRepository<EMRSystem.Emergency.EmergencyMaster.EmergencyMaster, long> emergencyMasterRepository,
                IRepository<EMRSystem.EmergencyChargeEntries.EmergencyChargeEntry, long> emergencyChargeEntriesRepository, IRepository<Prescriptions.Prescription, long> prescriptionRepository, IRepository<LabReports.PrescriptionLabTest, long> prescriptionLabTestRepository)
            : base(repository)
        {
            _patientRepository = patientRepository;
            _ipdChargeEntryRepository = ipdChargeEntryRepository;
            _emergencyMasterRepository = emergencyMasterRepository;
            _emergencyChargeEntriesRepository = emergencyChargeEntriesRepository;
            _prescriptionRepository = prescriptionRepository;
            _prescriptionLabTestRepository = prescriptionLabTestRepository;
        }

        protected override IQueryable<EmergencyCase> CreateFilteredQuery(PagedEmergencyCaseResultRequestDto input)
        {
            return Repository.GetAll()
                .Include(x => x.Patient)
                .Include(x => x.Doctor)
                .Include(x => x.Nurse)
                .WhereIf(!input.Keyword.IsNullOrWhiteSpace(),
                    x =>
                        x.Patient.FullName.Contains(input.Keyword) ||
                        x.Doctor.FullName.Contains(input.Keyword) ||
                        x.Nurse.FullName.Contains(input.Keyword) ||
                        x.EmergencyNumber.Contains(input.Keyword)
                );
        }

        public override async Task<EmergencyCaseDto> CreateAsync(CreateUpdateEmergencyCaseDto input)
        {
            input.ArrivalTime = DateTime.Now;
            var entity = ObjectMapper.Map<EmergencyCase>(input);

            entity.EmergencyNumber = $"ER-{DateTime.Now.Year}-{Guid.NewGuid().ToString("N").Substring(0, 4).ToUpper()}";

            var result = await Repository.InsertAndGetIdAsync(entity);
            var res = await _emergencyMasterRepository.GetAllAsync();
            var amount = await res.FirstOrDefaultAsync();

            if (input.Status == EmergencyStatus.Admitted)
            {
                var patient = await _patientRepository
                    .GetAllIncluding(p => p.Admissions)
                    .FirstOrDefaultAsync(p => p.Id == input.PatientId);

                if (patient == null)
                    throw new UserFriendlyException("Patient not found");

                //// Verify admissions exist
                if (!patient.Admissions.Any())
                    throw new UserFriendlyException("Patient is marked as admitted but has no admission records");

                // Get most recent ACTIVE admission
                var admission = patient.Admissions
                    .Where(a => !a.IsDischarged)
                    .OrderByDescending(a => a.AdmissionDateTime)
                    .FirstOrDefault();

                if (admission == null)
                    throw new UserFriendlyException("No active admission found for patient");

                var chargeEntry = new EMRSystem.IpdChargeEntry.IpdChargeEntry
                {
                    AdmissionId = admission.Id,
                    PatientId = patient.Id,
                    ChargeType = ChargeType.Other,
                    Quantity =1,
                    Description = $"Emergency Case Charge",
                    Amount = amount.Fee > 0 ? amount.Fee : 0
                };

                await _ipdChargeEntryRepository.InsertAsync(chargeEntry);

                // ✅ Mark patient as Emergency
                patient.IsEmergencyCharge = true;
                await _patientRepository.UpdateAsync(patient);
            }
            else
            {
                var chargeEntry = new EMRSystem.EmergencyChargeEntries.EmergencyChargeEntry
                {
                    PatientId = input.PatientId,
                    ChargeType = ChargeType.Other,
                    Quantity = 1,
                    Description = $"Emergency Case Charge",
                    Amount = amount.Fee > 0 ? amount.Fee : 0,
                    EmergencyCaseId = result,
                };

                await _emergencyChargeEntriesRepository.InsertAsync(chargeEntry);

                if (input.PatientId.HasValue)
                {
                    var patient = await _patientRepository.GetAsync(input.PatientId.Value);
                    if (patient != null)
                    {
                        // ✅ Mark patient as Emergency
                        patient.IsEmergencyCharge = true;
                        await _patientRepository.UpdateAsync(patient);
                    }
                }
            }

            await CurrentUnitOfWork.SaveChangesAsync();

            return MapToEntityDto(entity);
        }

        public async Task UpdateEmergencyCase(CreateUpdateEmergencyCaseDto input)
        {
            if (string.IsNullOrEmpty(input.EmergencyNumber))
            {
                input.EmergencyNumber = $"ER-{DateTime.Now.Year}-{Guid.NewGuid().ToString("N").Substring(0, 4).ToUpper()}";
            }

            // Update EmergencyCase
            var emergencyCase = ObjectMapper.Map<EmergencyCase>(input);
            await Repository.UpdateAsync(emergencyCase);

            if (input.PatientId.HasValue)
            {
                // ✅ EmergencyChargeEntry update
                var chargeEntries = await _emergencyChargeEntriesRepository
                    .GetAllListAsync(x => x.EmergencyCaseId == input.Id);

                foreach (var entry in chargeEntries)
                {
                    entry.PatientId = input.PatientId; // update patient
                    await _emergencyChargeEntriesRepository.UpdateAsync(entry);
                }

                // ✅ Prescription update
                var prescriptionEntries = await _prescriptionRepository
                    .GetAllListAsync(x => x.EmergencyCaseId == input.Id);

                foreach (var entry in prescriptionEntries)
                {
                    entry.PatientId = input.PatientId; // update patient
                    await _prescriptionRepository.UpdateAsync(entry);
                }

                // ✅ PrescriptionLabTests update
                var prescriptionLabTestsEntries = await _prescriptionLabTestRepository
                    .GetAllListAsync(x => x.EmergencyCaseId == input.Id);

                foreach (var entry in prescriptionLabTestsEntries)
                {
                    entry.PatientId = input.PatientId; // update patient
                    await _prescriptionLabTestRepository.UpdateAsync(entry);
                }

                var patient = await _patientRepository.GetAsync(input.PatientId.Value);
                if (patient != null)
                {
                    patient.IsEmergencyCharge = true;
                    await _patientRepository.UpdateAsync(patient);
                }
            }

            await CurrentUnitOfWork.SaveChangesAsync();
        }

    }
}
