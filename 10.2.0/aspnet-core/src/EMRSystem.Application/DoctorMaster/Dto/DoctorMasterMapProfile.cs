using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMRSystem.DoctorMaster.Dto
{
    public class DoctorMasterMapProfile : Profile
    {
        public DoctorMasterMapProfile()
        {
            CreateMap<EMRSystem.DoctorMaster.DoctorMaster, DoctorMasterDto>()
                .ForMember(dest => dest.Doctor, opt => opt.MapFrom(src => src.Doctor));
            CreateMap<EMRSystem.DoctorMaster.DoctorMaster, CreateUpdateDoctorMasterDto>().ReverseMap();
        }
    }
}
