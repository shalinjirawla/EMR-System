using AutoMapper;
using EMRSystem.LabReport.Dto;
using EMRSystem.LabReports;
using EMRSystem.PrescriptionLabTest.Dto;

namespace EMRSystem.PrescriptionLabTest.Dto
{
    public class PrescriptionLabTestMapProfile : Profile
    {
        public PrescriptionLabTestMapProfile()
        {
            // Parent ↔️ DTO
            CreateMap<EMRSystem.LabReports.PrescriptionLabTest, PrescriptionLabTestDto>()
                .ForMember(dest => dest.PatientName,
                           opt => opt.MapFrom(src => src.Patient.FullName))
                .ForMember(dest => dest.ReportTypeName,
                           opt => opt.MapFrom(src => src.LabReportsType.ReportType))
                // Map child collection to DTO collection
                .ForMember(dest => dest.ResultItems,
                           opt => opt.MapFrom(src => src.LabReportResultItems))
                .ReverseMap()
                // When mapping back, ignore the children so we can handle insertion manually
                .ForMember(dest => dest.LabReportResultItems, opt => opt.Ignore());

            // DTO → Create/Update DTO
            CreateMap<EMRSystem.LabReports.PrescriptionLabTest, CreateUpdatePrescriptionLabTestDto>()
                .ForMember(dest => dest.ResultItems,
                           opt => opt.MapFrom(src => src.LabReportResultItems))
                .ReverseMap()
                // Prevent AutoMapper from trying to overwrite your navigation collection
                .ForMember(dest => dest.LabReportResultItems, opt => opt.Ignore());

            // Individual ResultItem mappings
            CreateMap<LabReportResultItem, LabReportResultItemDto>()
                .ReverseMap();
        }
    }
}
