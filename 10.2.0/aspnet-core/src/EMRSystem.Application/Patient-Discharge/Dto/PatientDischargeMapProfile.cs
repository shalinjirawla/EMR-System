using AutoMapper;
using EMRSystem.Patient_Discharge.Dto;
using EMRSystem.PatientDischarge;
using System;
using System.Linq;

namespace EMRSystem.Patient_Discharge.Dto
{
    public class PatientDischargeMapProfile : Profile
    {
        public PatientDischargeMapProfile()
        {
            // 🔹 Base Discharge -> CreateUpdate DTO
            CreateMap<EMRSystem.PatientDischarge.PatientDischarge, CreateUpdatePatientDischargeDto>()
                .ForMember(dest => dest.AdmissionId, opt => opt.MapFrom(src => src.Admission != null ? src.Admission.Id : 0))
                .ForMember(dest => dest.PatientId, opt => opt.MapFrom(src => src.PatientId))
                .ForMember(dest => dest.DoctorId, opt => opt.MapFrom(src => src.DoctorId))
                .ForMember(dest => dest.DoctorName, opt => opt.MapFrom(src => src.Doctor != null ? src.Doctor.FullName : string.Empty))
                .ForMember(dest => dest.DoctorQualification, opt => opt.MapFrom(src => src.Doctor != null ? src.Doctor.Qualification : string.Empty))
                .ForMember(dest => dest.FollowUpDoctorName, opt => opt.MapFrom(src => src.FollowUpDoctor != null ? src.FollowUpDoctor.FullName : string.Empty));

            // 🔹 Patient + Admission -> PatientDetails DTO
            CreateMap<EMRSystem.PatientDischarge.PatientDischarge, PatientDetailsFordischargeSummaryDto>()
                .ForMember(dest => dest.PatientId, opt => opt.MapFrom(src => src.PatientId))
                .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.Patient.AbpUser.Name))
                .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.Patient.AbpUser.Surname))
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.Patient.AbpUser.UserName))
                .ForMember(dest => dest.EmailAddress, opt => opt.MapFrom(src => src.Patient.AbpUser.EmailAddress))
                .ForMember(dest => dest.MobileNumber, opt => opt.MapFrom(src => src.Patient.AbpUser.PhoneNumber))
                .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.Patient.FullName))
                .ForMember(dest => dest.Gender, opt => opt.MapFrom(src => src.Patient.Gender))
                .ForMember(dest => dest.Dob, opt => opt.MapFrom(src => src.Patient.DateOfBirth))
                .ForMember(dest => dest.Age, opt => opt.MapFrom(src => (int)((DateTime.Now - src.Patient.DateOfBirth).TotalDays / 365)))
                .ForMember(dest => dest.BloodGroup, opt => opt.MapFrom(src => src.Patient.BloodGroup))
                .ForMember(dest => dest.EmergencyNumber, opt => opt.MapFrom(src => src.Patient.EmergencyContactNumber))
                .ForMember(dest => dest.EmergencyPersonName, opt => opt.MapFrom(src => src.Patient.EmergencyContactName))
                .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.Patient.Address))
                .ForMember(dest => dest.DoctorName, opt => opt.MapFrom(src => src.Doctor != null ? src.Doctor.FullName : string.Empty))
                .ForMember(dest => dest.AdmissionDateTime, opt => opt.MapFrom(src => src.Admission.AdmissionDateTime))
                .ForMember(dest => dest.DischargeDateTime, opt => opt.MapFrom(src => src.DischargeDate ?? DateTime.Now))
                .ForMember(dest => dest.AdmissionId, opt => opt.MapFrom(src => src.Admission.Id))
                .ForMember(dest => dest.Room, opt => opt.MapFrom(src => src.Admission.Room != null ? src.Admission.Room.RoomNumber : "N/A"))
                .ForMember(dest => dest.BedNumber, opt => opt.MapFrom(src => src.Admission.Bed != null ? src.Admission.Bed.BedNumber : "N/A"))
                .ForMember(dest => dest.ReasonForAdmit, opt => opt.MapFrom(src => src.Admission.ReasonForAdmit));

            // 🔹 Full Discharge -> Combined DischargeSummary DTO
            CreateMap<EMRSystem.PatientDischarge.PatientDischarge, DischargeSummaryDto>()
                .ForMember(dest => dest.PatientDischarge, opt => opt.MapFrom(src => src))
                .ForMember(dest => dest.PatientDetails, opt => opt.MapFrom(src => src))
                .ForMember(dest => dest.Vitals, opt => opt.Ignore()) // fetched via app service
                .ForMember(dest => dest.Prescriptions, opt => opt.Ignore()) // fetched via app service
                .ForMember(dest => dest.PrescriptionLabTests, opt => opt.Ignore())
                .ForMember(dest => dest.SelectedEmergencyProcedures, opt => opt.Ignore())
                .ForMember(dest => dest.Invoices, opt => opt.Ignore());

            // 🔹 Discharge -> Simple DTO for Grid/List
            CreateMap<EMRSystem.PatientDischarge.PatientDischarge, PatientDischargeDto>()
                .ForMember(dest => dest.Admission, opt => opt.MapFrom(src => src.Admission))
                .ForMember(dest => dest.Patient, opt => opt.MapFrom(src => src.Patient))
                .ForMember(dest => dest.Doctor, opt => opt.MapFrom(src => src.Doctor))
                .ForMember(dest => dest.FollowUpDoctor, opt => opt.MapFrom(src => src.FollowUpDoctor));
        }
    }
}
