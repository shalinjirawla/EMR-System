using AutoMapper;
using EMRSystem.Appointments.Dto;
using EMRSystem.Appointments;
using EMRSystem.Nurse.Dto;

namespace EMRSystem.Patient_Discharge.Dto
{
    public class PatientDischargeMapProfile : Profile
    {
        public PatientDischargeMapProfile()
        {
            CreateMap<EMRSystem.PatientDischarge.PatientDischarge, PatientDischargeDto>()
                 .ForMember(dest => dest.TenantId, opt => opt.MapFrom(src => src.TenantId))
                .ForMember(dest => dest.Admission, opt => opt.MapFrom(src => src.Admission))
                .ForMember(dest => dest.Patient, opt => opt.MapFrom(src => src.Patient))
                .ForMember(dest => dest.Doctor, opt => opt.MapFrom(src => src.Doctor))
                .ForMember(dest => dest.BillingStaff, opt => opt.MapFrom(src => src.BillingStaff))
                .ForMember(dest => dest.PharmacistStaff, opt => opt.MapFrom(src => src.PharmacistStaff))
                .ForMember(dest => dest.DischargeDate, opt => opt.MapFrom(src => src.DischargeDate))
                .ForMember(dest => dest.DischargeSummary, opt => opt.MapFrom(src => src.DischargeSummary))
                .ForMember(dest => dest.DischargeStatus, opt => opt.MapFrom(src => src.DischargeStatus))
                .ReverseMap();


            CreateMap<EMRSystem.PatientDischarge.PatientDischarge, CreateUpdatePatientDischargeDto>()
                .ForMember(dest => dest.TenantId, opt => opt.MapFrom(src => src.TenantId))
                .ForMember(dest => dest.AdmissionId, opt => opt.MapFrom(src => src.Admission != null ? src.Admission.Id : (long?)null))
                .ForMember(dest => dest.PatientId, opt => opt.MapFrom(src => src.Patient != null ? src.Patient.Id : (long?)null))
                .ForMember(dest => dest.DischargeDate, opt => opt.MapFrom(src => src.DischargeDate))
                .ForMember(dest => dest.DischargeSummary, opt => opt.MapFrom(src => src.DischargeSummary))
                .ForMember(dest => dest.DischargeStatus, opt => opt.MapFrom(src => src.DischargeStatus))
                .ForMember(dest => dest.DoctorId, opt => opt.MapFrom(src => src.Doctor != null ? src.Doctor.Id : (long?)null))
                .ForMember(dest => dest.BillingStaffId, opt => opt.MapFrom(src => src.BillingStaffId))
                .ForMember(dest => dest.PharmacyStaffId, opt => opt.MapFrom(src => src.PharmacyStaffId))
                .ReverseMap();
        }
    }
}
