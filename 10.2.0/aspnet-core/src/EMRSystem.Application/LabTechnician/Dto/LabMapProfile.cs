using AutoMapper;
using EMRSystem.LabReport.Dto;

namespace EMRSystem.LabTechnician.Dto
{
    public class LabMapProfile : Profile
    {
        public LabMapProfile()
        {
            CreateMap<EMRSystem.LabReports.LabTechnician, CreateUpdateLabTechnicianDto>().ReverseMap();
            CreateMap<EMRSystem.LabReports.LabTechnician, LabTechniciansDto>().ReverseMap();
            CreateMap<LabReports.PrescriptionLabTest, LabRequestListDto>()
                .ForMember(dest => dest.PatientId, opt => opt.MapFrom(src => src.Prescription.PatientId))
                .ForMember(dest => dest.DoctorId, opt => opt.MapFrom(src => src.Prescription.DoctorId))
                .ForMember(dest => dest.PatientName, opt => opt.MapFrom(src => src.Prescription.Patient.FullName))
                .ForMember(dest => dest.DoctorName, opt => opt.MapFrom(src => src.Prescription.Doctor.FullName))
                .ForMember(dest => dest.LabReportTypeName, opt => opt.MapFrom(src => src.LabReportsType.ReportType))
                .ForMember(dest => dest.PrescriptionId, opt => opt.MapFrom(src => src.Prescription.Id))
                .ForMember(dest => dest.LabReportsTypeId, opt => opt.MapFrom(src => src.LabReportsTypeId))
                .ForMember(dest => dest.TestStatus, opt => opt.MapFrom(src => src.TestStatus));

            CreateMap<LabReports.PrescriptionLabTest, ViewLabReportDto>()
                            .ForMember(dest => dest.PrescriptionLabTestId, opt => opt.MapFrom(src => src.Id))
                            .ForMember(dest => dest.TestName, opt => opt.MapFrom(src => src.LabReportsType.ReportType))
                            .ForMember(dest => dest.PatientId, opt => opt.MapFrom(src => src.Prescription.Patient.Id))
                            .ForMember(dest => dest.PatientName, opt => opt.MapFrom(src => src.Prescription.Patient.FullName))
                            .ForMember(dest => dest.PatientDateOfBirth, opt => opt.MapFrom(src => src.Prescription.Patient.DateOfBirth))
                            .ForMember(dest => dest.Gender, opt => opt.MapFrom(src => src.Prescription.Patient.Gender))
                            .ForMember(dest => dest.DoctorName, opt => opt.MapFrom(src => src.Prescription.Doctor.FullName))
                            .ForMember(dest => dest.DoctorRegistrationNumber, opt => opt.MapFrom(src => src.Prescription.Doctor.RegistrationNumber))
                            .ForMember(dest => dest.LabReportResultItem, opt => opt.MapFrom(src => src.LabReportResultItems))
                            .ForMember(dest => dest.RecordedDate, opt => opt.MapFrom(src => src.CreatedDate));

            CreateMap<LabReports.PrescriptionLabTest, LabOrderListDto>()
                .ForMember(dest => dest.PatientName, opt => opt.MapFrom(src => src.Prescription.Patient.FullName))
                .ForMember(dest => dest.Gender, opt => opt.MapFrom(src => src.Prescription.Patient.Gender))
                .ForMember(dest => dest.AppointmentDate, opt => opt.MapFrom(src => src.Prescription.Appointment.AppointmentDate))
                .ForMember(dest => dest.DoctorName, opt => opt.MapFrom(src => src.Prescription.Doctor.FullName))
                .ForMember(dest => dest.LabReportTypeName, opt => opt.MapFrom(src => src.LabReportsType.ReportType))
                .ForMember(dest => dest.TestStatus, opt => opt.MapFrom(src => src.TestStatus))
                ;
        }
    }
}
