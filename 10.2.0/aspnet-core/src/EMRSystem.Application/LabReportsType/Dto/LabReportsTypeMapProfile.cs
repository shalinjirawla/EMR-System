using AutoMapper;
using EMRSystem.Doctor.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMRSystem.LabReportsType.Dto
{

    public class LabReportsTypeMapProfile : Profile
    {
        public LabReportsTypeMapProfile()
        {
            CreateMap<EMRSystem.LabReportsTypes.LabReportsType, LabReportsTypeDto>().ReverseMap();
            CreateMap<EMRSystem.LabReportsTypes.LabReportsType, CreateUpdateLabReportTypeDto>().ReverseMap();
        }
    }
}
