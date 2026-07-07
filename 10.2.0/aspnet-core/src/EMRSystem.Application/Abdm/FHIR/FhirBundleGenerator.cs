using Abp.Dependency;
using Abp.Domain.Repositories;
using AppointmentEntity = EMRSystem.Appointments.Appointment;
using DoctorEntity = EMRSystem.Doctors.Doctor;
using EMRSystem.LabReports;
using LabTestReceiptEntity = EMRSystem.LabTestReceipt.LabTestReceipt;
using EMRSystem.Patients;
using EMRSystem.Prescriptions;
using Hl7.Fhir.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Task = System.Threading.Tasks.Task;
using Condition = Hl7.Fhir.Model.Condition;
using Encounter = Hl7.Fhir.Model.Encounter;
using Patient = Hl7.Fhir.Model.Patient;

namespace EMRSystem.Abdm.FHIR
{
    public class FhirBundleGenerator : IFhirBundleGenerator, ITransientDependency
    {
        private readonly IRepository<Prescription, long> _prescriptionRepository;
        private readonly IRepository<LabTestReceiptEntity, long> _labReceiptRepository;
        private readonly IRepository<EMRSystem.Patients.Patient, long> _patientRepository;
        private readonly IRepository<DoctorEntity, long> _doctorRepository;

        public FhirBundleGenerator(
            IRepository<Prescription, long> prescriptionRepository,
            IRepository<LabTestReceiptEntity, long> labReceiptRepository,
            IRepository<EMRSystem.Patients.Patient, long> patientRepository,
            IRepository<DoctorEntity, long> doctorRepository)
        {
            _prescriptionRepository = prescriptionRepository;
            _labReceiptRepository = labReceiptRepository;
            _patientRepository = patientRepository;
            _doctorRepository = doctorRepository;
        }

        public async Task<Bundle> GeneratePrescriptionBundleAsync(long prescriptionId)
        {
            var prescription = await _prescriptionRepository.GetAll()
                .Include(p => p.Patient).ThenInclude(p => p.AbhaDetails)
                .Include(p => p.Doctor)
                .Include(p => p.Appointment)
                .Include(p => p.Items)
                .FirstOrDefaultAsync(p => p.Id == prescriptionId);

            if (prescription == null) throw new Exception("Prescription not found");

            var bundle = new Bundle
            {
                Id = Guid.NewGuid().ToString(),
                Type = Bundle.BundleType.Document,
                Timestamp = DateTimeOffset.Now,
                Identifier = new Identifier { System = "https://nha.gov.in/bundle", Value = prescription.Id.ToString() }
            };

            // 1. Composition
            var composition = CreateComposition(prescription.Patient, prescription.Doctor, prescription.Appointment, "Prescription Record");
            AddResourceToBundle(bundle, composition, composition.Id);

            // 2. Patient
            var patientResource = CreatePatientResource(prescription.Patient);
            AddResourceToBundle(bundle, patientResource, patientResource.Id);

            // 3. Practitioner
            var practitionerResource = CreatePractitionerResource(prescription.Doctor);
            AddResourceToBundle(bundle, practitionerResource, practitionerResource.Id);

            // 4. Encounter
            if (prescription.Appointment != null)
            {
                var encounter = CreateEncounterResource(prescription.Appointment, patientResource.Id, practitionerResource.Id);
                AddResourceToBundle(bundle, encounter, encounter.Id);
            }

            // 5. Condition (Diagnosis)
            if (!string.IsNullOrWhiteSpace(prescription.Diagnosis))
            {
                var condition = new Condition
                {
                    Id = Guid.NewGuid().ToString(),
                    ClinicalStatus = new CodeableConcept("http://terminology.hl7.org/CodeSystem/condition-clinical", "active"),
                    Subject = new ResourceReference($"Patient/{patientResource.Id}"),
                    Code = new CodeableConcept { Text = prescription.Diagnosis }
                };
                AddResourceToBundle(bundle, condition, condition.Id);
            }

            // 6. MedicationRequests
            foreach (var item in prescription.Items)
            {
                var medReq = new MedicationRequest
                {
                    Id = Guid.NewGuid().ToString(),
                    Status = MedicationRequest.MedicationrequestStatus.Active,
                    Intent = MedicationRequest.MedicationRequestIntent.Order,
                    Subject = new ResourceReference($"Patient/{patientResource.Id}"),
                    Medication = new CodeableConcept { Text = item.MedicineName },
                    DosageInstruction = new List<Dosage>
                    {
                        new Dosage { Text = $"{item.Frequency} for {item.NumberOfMedicine} medicines" }
                    }
                };
                AddResourceToBundle(bundle, medReq, medReq.Id);
            }

            return bundle;
        }

        public async Task<Bundle> GenerateDiagnosticReportBundleAsync(long labReportReceiptId)
        {
            var receipt = await _labReceiptRepository.GetAll()
                .Include(r => r.Patient).ThenInclude(p => p.AbhaDetails)
                .Include(r => r.PrescriptionLabTests).ThenInclude(plt => plt.LabReportResultItems)
                .FirstOrDefaultAsync(r => r.Id == labReportReceiptId);

            if (receipt == null) throw new Exception("Lab Receipt not found");

            var bundle = new Bundle
            {
                Id = Guid.NewGuid().ToString(),
                Type = Bundle.BundleType.Document,
                Timestamp = DateTimeOffset.Now
            };

            var composition = CreateComposition(receipt.Patient, null, null, "Diagnostic Report");
            AddResourceToBundle(bundle, composition, composition.Id);

            var patientResource = CreatePatientResource(receipt.Patient);
            AddResourceToBundle(bundle, patientResource, patientResource.Id);

            foreach (var plt in receipt.PrescriptionLabTests)
            {
                var diagnosticReport = new DiagnosticReport
                {
                    Id = Guid.NewGuid().ToString(),
                    Status = DiagnosticReport.DiagnosticReportStatus.Final,
                    Subject = new ResourceReference($"Patient/{patientResource.Id}"),
                    Issued = plt.CreatedDate,
                    Code = new CodeableConcept { Text = plt.LabReportsType?.ReportType ?? "General Lab Test" }
                };

                foreach (var result in plt.LabReportResultItems)
                {
                    var observation = new Observation
                    {
                        Id = Guid.NewGuid().ToString(),
                        Status = ObservationStatus.Final,
                        Subject = new ResourceReference($"Patient/{patientResource.Id}"),
                        Code = new CodeableConcept 
                        { 
                            Text = result.Test,
                            Coding = new List<Coding>()
                        },
                        Value = new FhirString(result.Result)
                    };
                    AddResourceToBundle(bundle, observation, observation.Id);
                    diagnosticReport.Result.Add(new ResourceReference($"Observation/{observation.Id}"));
                }

                AddResourceToBundle(bundle, diagnosticReport, diagnosticReport.Id);
            }

            return bundle;
        }

        private Composition CreateComposition(EMRSystem.Patients.Patient patient, DoctorEntity doctor, AppointmentEntity appointment, string title)
        {
            return new Composition
            {
                Id = Guid.NewGuid().ToString(),
                Status = CompositionStatus.Final,
                Type = new CodeableConcept("http://snomed.info/sct", "371530004", "Clinical consultation report"),
                Subject = new ResourceReference($"Patient/{patient.Id}"),
                Date = DateTime.Now.ToString("yyyy-MM-dd"),
                Title = title,
                Author = new List<ResourceReference> { new ResourceReference(doctor != null ? $"Practitioner/{doctor.Id}" : "Organization/1") }
            };
        }

        private Patient CreatePatientResource(EMRSystem.Patients.Patient patient)
        {
            var p = new Patient
            {
                Id = patient.Id.ToString(),
                Name = new List<HumanName> { new HumanName { Text = patient.FullName } },
                Gender = patient.Gender?.ToLower() == "male" ? AdministrativeGender.Male :
                         patient.Gender?.ToLower() == "female" ? AdministrativeGender.Female : AdministrativeGender.Unknown,
                BirthDate = patient.DateOfBirth.ToString("yyyy-MM-dd")
            };

            var abha = patient.AbhaDetails?.FirstOrDefault();
            if (abha != null && !string.IsNullOrEmpty(abha.AbhaNumber))
            {
                p.Identifier.Add(new Identifier
                {
                    System = "https://healthid.ndhm.gov.in",
                    Value = abha.AbhaNumber
                });
            }

            return p;
        }

        private Practitioner CreatePractitionerResource(DoctorEntity doctor)
        {
            if (doctor == null) return new Practitioner { Id = Guid.NewGuid().ToString() };

            var practitioner = new Practitioner
            {
                Id = doctor.Id.ToString(),
                Name = new List<HumanName> { new HumanName { Text = doctor.FullName } }
            };

            if (!string.IsNullOrEmpty(doctor.HprId))
            {
                practitioner.Identifier.Add(new Identifier
                {
                    System = "https://hpr.ndhm.gov.in",
                    Value = doctor.HprId
                });
            }

            return practitioner;
        }

        private Encounter CreateEncounterResource(AppointmentEntity appointment, string patientId, string practitionerId)
        {
            return new Encounter
            {
                Id = appointment.Id.ToString(),
                Status = appointment.Status == EMRSystem.Appointments.AppointmentStatus.Completed ? Encounter.EncounterStatus.Finished : Encounter.EncounterStatus.Planned,
                Class = new Coding("http://terminology.hl7.org/CodeSystem/v3-ActCode", "AMB", "ambulatory"),
                Subject = new ResourceReference($"Patient/{patientId}"),
                Participant = new List<Encounter.ParticipantComponent>
                {
                    new Encounter.ParticipantComponent
                    {
                        Individual = new ResourceReference($"Practitioner/{practitionerId}")
                    }
                },
                Period = new Period { Start = appointment.AppointmentDate.ToString("yyyy-MM-ddTHH:mm:ssZ") }
            };
        }

        private void AddResourceToBundle(Bundle bundle, Resource resource, string fullUrlSuffix)
        {
            bundle.Entry.Add(new Bundle.EntryComponent
            {
                FullUrl = $"urn:uuid:{fullUrlSuffix}",
                Resource = resource
            });
        }
    }
}
