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
        }
    }
}
