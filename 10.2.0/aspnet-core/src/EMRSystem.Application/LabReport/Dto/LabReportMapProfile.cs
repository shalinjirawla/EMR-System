using AutoMapper;
using EMRSystem.LabReports;
using EMRSystem.LabReportsType.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMRSystem.LabReport.Dto
{
    public class LabReportMapProfile : Profile
    {
        public LabReportMapProfile()
        {
            CreateMap<EMRSystem.LabReports.LabReportResultItem, LabReportResultItemDto>().ReverseMap();
            CreateMap<EMRSystem.LabReports.LabReportResultItem, CreateUpdateLabReportResultItemDto>().ReverseMap();
        }
    }
}
