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
                            .ForMember(dest => dest.PatientId,
                                       opt => opt.MapFrom(src =>
                                           src.Prescription != null
                                               ? src.Prescription.PatientId
                                               : src.PatientId))
                            .ForMember(dest => dest.PatientName,
                                       opt => opt.MapFrom(src =>
                                           src.Prescription != null
                                               ? src.Prescription.Patient.FullName
                                               : src.Patient.FullName))
                            .ForMember(dest => dest.DoctorId,
                                       opt => opt.MapFrom(src => src.Prescription.DoctorId))
                            .ForMember(dest => dest.DoctorName,
                                       opt => opt.MapFrom(src => src.Prescription.Doctor.FullName))
                            .ForMember(dest => dest.PrescriptionId,
                                       opt => opt.MapFrom(src => src.PrescriptionId))
                            .ForMember(dest => dest.LabReportsTypeId,
                                       opt => opt.MapFrom(src => src.LabReportsTypeId))
                            .ForMember(dest => dest.LabReportTypeName,
                                       opt => opt.MapFrom(src => src.LabReportsType.ReportType))
                            .ForMember(dest => dest.TestStatus,
                                       opt => opt.MapFrom(src => src.TestStatus));


            CreateMap<EMRSystem.LabReports.PrescriptionLabTest, ViewLabReportDto>()
                             .ForMember(d => d.PrescriptionLabTestId,
                                        opt => opt.MapFrom(src => src.Id))
                             .ForMember(d => d.TestName,
                                        opt => opt.MapFrom(src => src.LabReportsType.ReportType))

                             // FALLBACK logic: if Prescription exists, grab from there; otherwise use direct Patient nav
                             .ForMember(d => d.PatientId,
                                        opt => opt.MapFrom(src =>
                                            src.Prescription != null
                                              ? src.Prescription.PatientId
                                              : src.PatientId))
                             .ForMember(d => d.PatientName,
                                        opt => opt.MapFrom(src =>
                                            src.Prescription != null
                                              ? src.Prescription.Patient.FullName
                                              : src.Patient.FullName))
                             .ForMember(d => d.PatientDateOfBirth,
                                        opt => opt.MapFrom(src =>
                                            (src.Prescription != null
                                              ? src.Prescription.Patient.DateOfBirth
                                              : src.Patient.DateOfBirth)
                                            .ToString("yyyy-MM-dd")))     // format as needed
                             .ForMember(d => d.Gender,
                                        opt => opt.MapFrom(src =>
                                            src.Prescription != null
                                              ? src.Prescription.Patient.Gender
                                              : src.Patient.Gender))

                             .ForMember(d => d.DoctorName,
                                        opt => opt.MapFrom(src => src.Prescription.Doctor.FullName))
                             .ForMember(d => d.DoctorRegistrationNumber,
                                        opt => opt.MapFrom(src => src.Prescription.Doctor.RegistrationNumber))

                             .ForMember(d => d.LabReportResultItem,
                                        opt => opt.MapFrom(src => src.LabReportResultItems))
                             .ForMember(d => d.RecordedDate,
                                        opt => opt.MapFrom(src => src.CreatedDate.Value));


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
