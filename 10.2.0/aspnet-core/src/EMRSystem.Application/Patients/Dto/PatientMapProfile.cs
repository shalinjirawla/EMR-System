using AutoMapper;
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
              .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.Address))
              .ForMember(dest => dest.BloodGroup, opt => opt.MapFrom(src => src.BloodGroup))
              .ForMember(dest => dest.EmergencyContactName, opt => opt.MapFrom(src => src.EmergencyContactName))
              .ForMember(dest => dest.EmergencyContactNumber, opt => opt.MapFrom(src => src.EmergencyContactNumber))
               .ForMember(dest => dest.AssignedNurseId, opt => opt.MapFrom(src => src.Nurses != null ? src.Nurses.Id : (long?)null))
               .ForMember(dest => dest.AssignedDoctorId, opt => opt.MapFrom(src => src.Doctors != null ? src.Doctors.Id : (long?)null))
               .ForMember(dest => dest.NurseName, opt => opt.MapFrom(src => src.Nurses != null ? src.Nurses.FullName : null))
               .ForMember(dest => dest.DoctorName, opt => opt.MapFrom(src => src.Doctors != null ? src.Doctors.FullName : null))
              .ForMember(dest => dest.AbpUserId, opt => opt.MapFrom(src => src.AbpUserId))
              .ForMember(dest => dest.IsAdmitted, opt => opt.MapFrom(src => src.IsAdmitted))
              .ForMember(dest => dest.AdmissionDate, opt => opt.MapFrom(src => src.AdmissionDate))
              .ForMember(dest => dest.DischargeDate, opt => opt.MapFrom(src => src.DischargeDate))
              .ForMember(dest => dest.InsuranceProvider, opt => opt.MapFrom(src => src.InsuranceProvider))
              .ForMember(dest => dest.InsurancePolicyNumber, opt => opt.MapFrom(src => src.InsurancePolicyNumber))
              .ReverseMap();
        }
    }
}
