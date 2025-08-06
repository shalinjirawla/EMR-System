using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMRSystem.LabMasters.Dto.LabReportTypeItem
{
    public class LabReportTypeItemMapProfile : Profile
    {
        public LabReportTypeItemMapProfile()
        {
            CreateMap<EMRSystem.LabMasters.LabReportTypeItem, LabReportTypeItemDto>().ReverseMap();
            CreateMap<EMRSystem.LabMasters.LabReportTypeItem, CreateUpdateLabReportTypeItemDto>().ReverseMap();
        }
    }
}
