using AutoMapper;
using EMRSystem.Nurse.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMRSystem.Pharmacist.Dto
{
    public class PharmacistMapProfile : Profile
    {
        public PharmacistMapProfile()
        {
            CreateMap<EMRSystem.Pharmacists.Pharmacist, PharmacistDto>().ReverseMap();
            CreateMap<EMRSystem.Pharmacists.Pharmacist, CreateUpdatePharmacistDto>().ReverseMap();
        }
    }
}
