using AutoMapper;
using EMRSystem.Nurse.Dto;
using EMRSystem.Pharmacists;
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

            CreateMap<PharmacistInventory, PharmacistInventoryDto>()
                .ForMember(dest => dest.StockStatus, opt => opt.Ignore())
                .ForMember(dest => dest.ExpiryStatus, opt => opt.Ignore())
                .ReverseMap();

            CreateMap<PharmacistInventory, CreateUpdatePharmacistInventoryDto>().ReverseMap();
        }
    }
}
