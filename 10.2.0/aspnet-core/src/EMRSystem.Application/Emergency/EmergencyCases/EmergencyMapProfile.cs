using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMRSystem.Emergency.EmergencyCase
{
    public class EmergencyMapProfile : Profile
    {
        public EmergencyMapProfile()
        {
            CreateMap<EmergencyCase, EmergencyCaseDto>()
            .ForMember(d => d.PatientName, opt => opt.MapFrom(s => s.Patient != null ? s.Patient.FullName : ""))
            .ForMember(d => d.DoctorName, opt => opt.MapFrom(s => s.Doctor != null ? s.Doctor.FullName : ""))
            .ForMember(d => d.NurseName, opt => opt.MapFrom(s => s.Nurse != null ? s.Nurse.FullName : ""));
            CreateMap<CreateUpdateEmergencyCaseDto, EmergencyCase>();
        }
    }
}
