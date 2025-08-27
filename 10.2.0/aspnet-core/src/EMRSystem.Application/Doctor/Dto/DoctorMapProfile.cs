using EMRSystem.Patients.Dto;
using EMRSystem.Patients;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;

namespace EMRSystem.Doctor.Dto
{
    public class DoctorMapProfile : Profile
    {
        public DoctorMapProfile()
        {
            CreateMap<EMRSystem.Doctors.Doctor, DoctorDto>()
                 .ForMember(dest => dest.Department, opt => opt.MapFrom(src => src.Department)); // Map Department

            CreateMap<EMRSystem.Doctors.Doctor, CreateUpdateDoctorDto>().ReverseMap();
        }
    }
}
