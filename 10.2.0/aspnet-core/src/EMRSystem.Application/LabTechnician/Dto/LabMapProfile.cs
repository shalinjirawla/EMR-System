using AutoMapper;
using EMRSystem.LabReports;

namespace EMRSystem.LabTechnician.Dto
{
    public class LabMapProfile : Profile
    {
        public LabMapProfile()
        {
            CreateMap<LabReport, CreateUpdateLabReportDto>().ReverseMap();
            CreateMap<LabReport, LabReportDto>().ReverseMap();
            CreateMap<EMRSystem.LabReports.LabTechnician, CreateUpdateLabTechnicianDto>().ReverseMap();
            CreateMap<EMRSystem.LabReports.LabTechnician, LabTechniciansDto>().ReverseMap();
        }
    }
}
