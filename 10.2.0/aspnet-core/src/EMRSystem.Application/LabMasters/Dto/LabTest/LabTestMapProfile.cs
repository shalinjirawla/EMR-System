using AutoMapper;
using EMRSystem.LabMasters.Dto.MeasureUnit;
using EMRSystem.LabMasters.Dto.TestResultLimit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMRSystem.LabMasters.Dto.LabTest
{
    public class LabTestMapProfile : Profile
    {
        public LabTestMapProfile()
        {
            CreateMap<EMRSystem.LabMasters.LabTest, LabTestDto>()
                .ForMember(dest => dest.MeasureUnitName, opt => opt.MapFrom(src => src.MeasureUnit.Name));

            CreateMap<CreateUpdateLabTestDto, EMRSystem.LabMasters.LabTest>()
               .ForMember(d => d.TenantId, o => o.MapFrom(s => s.TenantId))
               .ForMember(d => d.MeasureUnitId,o=>o.MapFrom(s => s.MeasureUnitId))
               .ReverseMap();
        }
    }
}
