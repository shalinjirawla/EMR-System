using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMRSystem.LabMasters.Dto.MeasureUnit
{
    public class MeasureUnitMapProfile : Profile
    {
        public MeasureUnitMapProfile()
        {
            CreateMap<EMRSystem.LabMasters.MeasureUnit, MeasureUnitDto>().ReverseMap();
            CreateMap<CreateUpdateMeasureUnitDto, EMRSystem.LabMasters.MeasureUnit>()
               .ForMember(d => d.TenantId, o => o.MapFrom(s => s.TenantId))
               .ReverseMap();
        }
    }
}
