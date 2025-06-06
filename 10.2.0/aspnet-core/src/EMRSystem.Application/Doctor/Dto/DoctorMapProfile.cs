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
            CreateMap<EMRSystem.Doctors.Doctor, DoctorDto>().ReverseMap();
            CreateMap<EMRSystem.Doctors.Doctor, CreateUpdateDoctorDto>().ReverseMap();
        }
    }
}
