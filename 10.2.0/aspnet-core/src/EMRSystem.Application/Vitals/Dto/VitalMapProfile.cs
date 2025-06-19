using AutoMapper;
using EMRSystem.Nurse.Dto;
using EMRSystem.Patients;
using EMRSystem.Patients.Dto;
using EMRSystem.Prescriptions;
using EMRSystem.Prescriptions.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMRSystem.Vitals.Dto
{
    public class VitalMapProfile : Profile
    {
        public VitalMapProfile()
        {
            CreateMap<Vital, VitalDto>()
      .ForMember(dest => dest.Patient, opt => opt.MapFrom(src => src.Patient))
      .ForMember(dest => dest.Nurse, opt => opt.MapFrom(src => src.Nurse));

            CreateMap<Vital, CreateUpdateVitalDto>().ReverseMap();
            CreateMap<Patient, PatientDto>();
            CreateMap<EMRSystem.Nurses.Nurse, NurseDto>();
        }
    }
}
