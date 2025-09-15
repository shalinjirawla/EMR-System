using AutoMapper;
using EMRSystem.Appointments;
using EMRSystem.Patient_Discharge.Dto;
using EMRSystem.Prescriptions;
using EMRSystem.Vitals;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMRSystem.Patients.Dto
{
    public class PatientMapProfile : Profile
    {
        public PatientMapProfile()
        {
            CreateMap<Patient, PatientDropDownDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.FullName))
                .ForMember(dest => dest.IsAdmitted, opt => opt.MapFrom(src => src.IsAdmitted));

            CreateMap<Patient, PatientDto>().ReverseMap();
            CreateMap<Patient, CreateUpdatePatientDto>().ReverseMap();
            CreateMap<Patient, PatientsForDoctorAndNurseDto>()
              .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
              .ForMember(dest => dest.EmailAddress, opt => opt.MapFrom(src => src.AbpUser.EmailAddress))
              .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.AbpUser.IsActive))
              .ForMember(dest => dest.TenantId, opt => opt.MapFrom(src => src.TenantId))
              .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.FullName))
              .ForMember(dest => dest.DateOfBirth, opt => opt.MapFrom(src => src.DateOfBirth))
              .ForMember(dest => dest.Gender, opt => opt.MapFrom(src => src.Gender))
              .ForMember(dest => dest.IsAdmitted, opt => opt.MapFrom(src => src.IsAdmitted))
              .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.Address))
              .ForMember(dest => dest.BloodGroup, opt => opt.MapFrom(src => src.BloodGroup))
              .ForMember(dest => dest.EmergencyContactName, opt => opt.MapFrom(src => src.EmergencyContactName))
              .ForMember(dest => dest.Discharge_Status, opt => opt.MapFrom(src => src._PatientDischarge.Select(x => x.DischargeStatus).FirstOrDefault()))
              .ForMember(d => d.AssignedDoctorId,
               opt => opt.MapFrom(s => s.Admissions
                   .OrderByDescending(a => a.AdmissionDateTime)
                   .Select(a => (long?)a.DoctorId)
                   .FirstOrDefault()))
    .ForMember(d => d.DoctorName,
               opt => opt.MapFrom(s => s.Admissions
                   .OrderByDescending(a => a.AdmissionDateTime)
                   .Select(a => a.Doctor.FullName)
                   .FirstOrDefault()))

    .ForMember(d => d.AssignedNurseId,
               opt => opt.MapFrom(s => s.Admissions
                   .OrderByDescending(a => a.AdmissionDateTime)
                   .Select(a => a.NurseId)
                   .FirstOrDefault()))
    .ForMember(d => d.NurseName,
               opt => opt.MapFrom(s => s.Admissions
                   .OrderByDescending(a => a.AdmissionDateTime)
                   .Select(a => a.Nurse.FullName)
                   .FirstOrDefault()))
              //.ForMember(dest => dest.EmergencyContactNumber, opt => opt.MapFrom(src => src.EmergencyContactNumber))
              // .ForMember(dest => dest.AssignedNurseId, opt => opt.MapFrom(src => src.Nurses != null ? src.Nurses.Id : (long?)null))
              // .ForMember(dest => dest.AssignedDoctorId, opt => opt.MapFrom(src => src.Doctors != null ? src.Doctors.Id : (long?)null))
              // .ForMember(dest => dest.NurseName, opt => opt.MapFrom(src => src.Nurses != null ? src.Nurses.FullName : null))
              // .ForMember(dest => dest.DoctorName, opt => opt.MapFrom(src => src.Doctors != null ? src.Doctors.FullName : null))
              .ForMember(dest => dest.AbpUserId, opt => opt.MapFrom(src => src.AbpUserId))
              //.ForMember(dest => dest.IsAdmitted, opt => opt.MapFrom(src => src.IsAdmitted))
              //.ForMember(dest => dest.AdmissionDate, opt => opt.MapFrom(src => src.AdmissionDate))
              //.ForMember(dest => dest.DischargeDate, opt => opt.MapFrom(src => src.DischargeDate))
              //.ForMember(dest => dest.InsuranceProvider, opt => opt.MapFrom(src => src.InsuranceProvider))
              //.ForMember(dest => dest.InsurancePolicyNumber, opt => opt.MapFrom(src => src.InsurancePolicyNumber))
              .ReverseMap();


            CreateMap<Patient, PatientDetailsAndMedicalHistoryDto>()
                 .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                 .ForMember(dest => dest.TenantId, opt => opt.MapFrom(src => src.TenantId))
                 .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.FullName))
                 .ForMember(dest => dest.DateOfBirth, opt => opt.MapFrom(src => src.DateOfBirth))
                 .ForMember(dest => dest.Gender, opt => opt.MapFrom(src => src.Gender))
                 .ForMember(dest => dest.EmailAddress, opt => opt.MapFrom(src => src.AbpUser.EmailAddress))
                 .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.AbpUser.PhoneNumber))
                 .ForMember(dest => dest.BloodGroup, opt => opt.MapFrom(src => src.BloodGroup))
                 .ForMember(dest => dest.EmergencyContactName, opt => opt.MapFrom(src => src.EmergencyContactName))
                 .ForMember(dest => dest.EmergencyContactNumber, opt => opt.MapFrom(src => src.EmergencyContactNumber))
                 .ForMember(dest => dest.AssignedDoctorId,
                            opt => opt.MapFrom(src => src.Admissions
                                .OrderByDescending(a => a.AdmissionDateTime)
                                .Select(a => (long?)a.DoctorId)
                                .FirstOrDefault()))
                 .ForMember(dest => dest.AssignedDoctorName,
                            opt => opt.MapFrom(src => src.Admissions
                                .OrderByDescending(a => a.AdmissionDateTime)
                                .Select(a => a.Doctor.FullName)
                                .FirstOrDefault()))
                 .ForMember(dest => dest.AssignedNurseId,
                            opt => opt.MapFrom(src => src.Admissions
                                .OrderByDescending(a => a.AdmissionDateTime)
                                .Select(a => (long?)a.NurseId)
                                .FirstOrDefault()))
                 .ForMember(dest => dest.AssignedNurseName,
                            opt => opt.MapFrom(src => src.Admissions
                                .OrderByDescending(a => a.AdmissionDateTime)
                                .Select(a => a.Nurse.FullName)
                                .FirstOrDefault()))
                 //.ForMember(dest => dest.IsAdmitted, opt => opt.MapFrom(src => src.IsAdmitted))
                 //.ForMember(dest => dest.AdmissionDate, opt => opt.MapFrom(src => src.AdmissionDate))
                 //.ForMember(dest => dest.DischargeDate, opt => opt.MapFrom(src => src.DischargeDate))
                 //.ForMember(dest => dest.InsuranceProvider, opt => opt.MapFrom(src => src.InsuranceProvider))
                 //.ForMember(dest => dest.InsurancePolicyNumber, opt => opt.MapFrom(src => src.InsurancePolicyNumber))
                 .ForMember(dest => dest.AbpUserId, opt => opt.MapFrom(src => src.AbpUserId))
                 //.ForMember(dest => dest.AssignedNurseId, opt => opt.MapFrom(src => src.AssignedNurseId))
                 //.ForMember(dest => dest.AssignedNurseName, opt => opt.MapFrom(src => src.Nurses.FullName))
                 //.ForMember(dest => dest.AssignedDoctorId, opt => opt.MapFrom(src => src.AssignedDoctorId))
                 //.ForMember(dest => dest.AssignedDoctorName, opt => opt.MapFrom(src => src.Doctors.FullName))
                 .ForMember(dest => dest.patientVitalsDetails, opt => opt.MapFrom(src => src.Vitals.FirstOrDefault()))
                 .ForMember(dest => dest.patientPrescriptionsHistory, opt => opt.MapFrom(src => src.Prescriptions))
                 .ForMember(dest => dest.patientAppointmentHistory, opt => opt.MapFrom(src => src.Appointments));

            CreateMap<Vital, PatientVitalsDetailsDto>()
                .ForMember(dest => dest.BloodPressure, opt => opt.MapFrom(src => src.BloodPressure))
                .ForMember(dest => dest.HeartRate, opt => opt.MapFrom(src => src.HeartRate))
                .ForMember(dest => dest.RespirationRate, opt => opt.MapFrom(src => src.RespirationRate))
                .ForMember(dest => dest.Temperature, opt => opt.MapFrom(src => src.Temperature))
                .ForMember(dest => dest.OxygenSaturation, opt => opt.MapFrom(src => src.OxygenSaturation))
                .ForMember(dest => dest.Height, opt => opt.MapFrom(src => src.Height))
                .ForMember(dest => dest.Weight, opt => opt.MapFrom(src => src.Weight))
                .ForMember(dest => dest.BMI, opt => opt.MapFrom(src => src.BMI))
                .ForMember(dest => dest.DateRecorded, opt => opt.MapFrom(src => src.DateRecorded));

            CreateMap<Appointment, PatientAppointmentHistoryDto>()
                .ForMember(dest => dest.AppointmentDate, opt => opt.MapFrom(src => src.AppointmentDate))
                //.ForMember(dest => dest.StartTime, opt => opt.MapFrom(src => src.StartTime))
                .ForMember(dest => dest.IsFollowUp, opt => opt.MapFrom(src => src.IsFollowUp))
                .ForMember(dest => dest.DoctorId, opt => opt.MapFrom(src => src.DoctorId))
                .ForMember(dest => dest.DoctorName, opt => opt.MapFrom(src => src.Doctor.FullName))
                //.ForMember(dest => dest.NurseId, opt => opt.MapFrom(src => src.NurseId))
                //.ForMember(dest => dest.NurseName, opt => opt.MapFrom(src => src.Nurse.FullName))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status))
                .ForMember(dest => dest.ReasonForVisit, opt => opt.MapFrom(src => src.ReasonForVisit));

            CreateMap<Prescription, PatientPrescriptionsHistoryDto>()
                .ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.Items))
                .ForMember(dest => dest.DoctorId, opt => opt.MapFrom(src => src.Doctor.Id))
                .ForMember(dest => dest.DoctorName, opt => opt.MapFrom(src => src.Doctor.FullName));

            CreateMap<PrescriptionItem, PatientPrescriptionsItemHistoryDto>()
                  .ForMember(dest => dest.MedicineName, opt => opt.MapFrom(src => src.MedicineName))
                  .ForMember(dest => dest.Dosage, opt => opt.MapFrom(src => src.Dosage))
                  .ForMember(dest => dest.Frequency, opt => opt.MapFrom(src => src.Frequency))
                  .ForMember(dest => dest.Duration, opt => opt.MapFrom(src => src.Duration))
                  .ForMember(dest => dest.PrescriptionId, opt => opt.MapFrom(src => src.PrescriptionId));


            //CreateMap<Vital, TreatmentSummaryForDischargeSummaryDto>()
            //.ForMember(dest => dest.LastVitalDateRecorded, opt => opt.MapFrom(src => src.DateRecorded))
            //.ForMember(dest => dest.BloodPressure, opt => opt.MapFrom(src => src.BloodPressure))
            //.ForMember(dest => dest.HeartRate, opt => opt.MapFrom(src => src.HeartRate))
            //.ForMember(dest => dest.RespirationRate, opt => opt.MapFrom(src => src.RespirationRate))
            //.ForMember(dest => dest.Temperature, opt => opt.MapFrom(src => src.Temperature))
            //.ForMember(dest => dest.OxygenSaturation, opt => opt.MapFrom(src => src.OxygenSaturation))
            //.ForMember(dest => dest.Height, opt => opt.MapFrom(src => src.Height))
            //.ForMember(dest => dest.Weight, opt => opt.MapFrom(src => src.Weight))
            //.ForMember(dest => dest.BMI, opt => opt.MapFrom(src => src.BMI))
            //.ForMember(dest => dest.VitalNotes, opt => opt.MapFrom(src => src.Notes));

        }
    }
}
