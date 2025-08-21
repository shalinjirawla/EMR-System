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
                 .ForMember(d => d.EmergencyNumber, opt => opt.MapFrom(s => s.EmergencyCase != null ? s.EmergencyCase.EmergencyNumber : ""));
            CreateMap<CreateUpdateTriageDto, Triage>();
        }
    }
}
