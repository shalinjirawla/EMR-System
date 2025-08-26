using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMRSystem.Emergency.Triage
{
    public class TriageApplicationAutoMapperProfile : Profile
    {
        public TriageApplicationAutoMapperProfile()
        {
            CreateMap<Triage, TriageDto>()
                    .ForMember(dest => dest.NurseName,
                        opt => opt.MapFrom(src => src.Nurse != null ? src.Nurse.FullName : null))
                    .ForMember(dest => dest.EmergencyNumber,
                        opt => opt.MapFrom(src => src.EmergencyCase != null ? src.EmergencyCase.EmergencyNumber : null))
                .ReverseMap();

            CreateMap<CreateUpdateTriageDto, Triage>().ReverseMap();
        }
    }
}
