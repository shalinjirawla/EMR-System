using AutoMapper;
using EMRSystem.Prescriptions.Dto;
using EMRSystem.Prescriptions;
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
            CreateMap<Vital, VitalDto>().ReverseMap();
            CreateMap<Vital, CreateUpdateVitalDto>().ReverseMap();
        }
    }
}
