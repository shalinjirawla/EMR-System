using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMRSystem.LabReportTemplateItem.Dto
{
    public class LabReportTemplateItemMapProfile : Profile
    {
        public LabReportTemplateItemMapProfile()
        {
            CreateMap<LabReportTemplateItem, LabReportTemplateItemDto>()
                .ForMember(dest => dest.LabReportsTypeName, opt => opt.MapFrom(src => src.LabReportsType.ReportType));

            CreateMap<CreateUpdateLabReportTemplateItemDto, LabReportTemplateItem>();
        }
    }
}
