using AutoMapper;
using EMRSystem.Insurances.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMRSystem.Admissions.Dto
{
    public class AdmissionMapProfile : Profile
    {
        public AdmissionMapProfile()
        {
            CreateMap<EMRSystem.Admission.Admission, AdmissionDto>()
                .ForMember(dest => dest.IsDischarged, opt => opt.MapFrom(src => src.IsDischarged))
                .ForMember(dest => dest.PatientName, opt => opt.MapFrom(src => src.Patient != null ? src.Patient.FullName : null))
                .ForMember(dest => dest.DoctorName, opt => opt.MapFrom(src => src.Doctor != null ? src.Doctor.FullName : null))
                .ForMember(dest => dest.NurseName, opt => opt.MapFrom(src => src.Nurse != null ? src.Nurse.FullName : null))
                .ForMember(dest => dest.RoomNumber, opt => opt.MapFrom(src => src.Room != null ? src.Room.RoomNumber : null))
                .ForMember(dest => dest.BedNumber, opt => opt.MapFrom(src => src.Bed != null ? src.Bed.BedNumber : null))
                .ForMember(dest => dest.RoomTypeName, opt => opt.MapFrom(src => src.Room != null && src.Room.RoomTypeMaster != null ? src.Room.RoomTypeMaster.TypeName : null))
                .ForMember(dest => dest.RoomTypePricePerDay, opt => opt.MapFrom(src => src.Room != null && src.Room.RoomTypeMaster != null ? src.Room.RoomTypeMaster.DefaultPricePerDay : (decimal?)null))
                .ForMember(dest => dest.InsuranceName, opt => opt.MapFrom(src => src.PatientInsurance != null ? src.PatientInsurance.InsuranceMaster.InsuranceName : null))
                .ForMember(dest => dest.InsuranceId, opt => opt.MapFrom(src => src.PatientInsurance != null ?(long?)src.PatientInsurance.InsuranceId: null))
                .ForMember(dest => dest.PolicyNumber, opt => opt.MapFrom(src => src.PatientInsurance != null ? src.PatientInsurance.PolicyNumber : null))
                .ForMember(dest => dest.CoPayPercentage, opt => opt.MapFrom(src => src.PatientInsurance != null ? src.PatientInsurance.CoPayPercentage : null))
                .ForMember(dest => dest.CoverageLimit,opt => opt.MapFrom(src => src.PatientInsurance != null? (decimal?)src.PatientInsurance.CoverageLimit
        : null));

            CreateMap<CreateUpdateAdmissionDto, EMRSystem.Admission.Admission>()
                .ForMember(dest => dest.PatientInsurance, opt => opt.Ignore());

            // Nested Insurance Mapping (ignore PatientId)
            CreateMap<CreateUpdatePatientInsuranceDto, EMRSystem.Insurances.PatientInsurance>()
                .ForMember(dest => dest.PatientId, opt => opt.Ignore());
        }
    }

}
