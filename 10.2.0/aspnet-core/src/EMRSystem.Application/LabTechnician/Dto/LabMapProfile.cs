using AutoMapper;
using EMRSystem.Patients.Dto;
using EMRSystem.Patients;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EMRSystem.LabReports;

namespace EMRSystem.LabTechnician.Dto
{
    public class LabMapProfile : Profile
    {
        public LabMapProfile()
        {
            CreateMap<LabReport, LabDto>().ReverseMap();
            CreateMap<LabReport, CreateUpdateLabDto>().ReverseMap();
        }
    }
}
