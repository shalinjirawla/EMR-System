using AutoMapper;
using EMRSystem.Doctor.Dto;
using EMRSystem.EmergencyProcedure.Dto;
using EMRSystem.LabTechnician.Dto;
using EMRSystem.Patient_Discharge.Dto;
using EMRSystem.PrescriptionLabTest.Dto;
using EMRSystem.Prescriptions.Dto;
using EMRSystem.Vitals;
using EMRSystem.Vitals.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace EMRSystem.Prescriptions.Dto
{
    //class PrescriptionMapProfile : Profile
    //{
    //    public PrescriptionMapProfile()
    //    {
    //        CreateMap<Prescription, PrescriptionDto>().ReverseMap();
    //        CreateMap<CreateUpdatePrescriptionDto, Prescription>();
    //        CreateMap<Prescription, CreateUpdatePrescriptionDto>()
    //           .ForMember(dest => dest.AppointmentId, opt => opt.MapFrom(src => src.Appointment.Id))
    //           .ForMember(dest => dest.DoctorId, opt => opt.MapFrom(src => src.Doctor.Id))
    //           .ForMember(dest => dest.PatientId, opt => opt.MapFrom(src => src.Patient.Id));

    //        CreateMap<PrescriptionItem, PrescriptionItemDto>().ReverseMap();
    //        CreateMap<PrescriptionItem, CreateUpdatePrescriptionItemDto>().ReverseMap();
    //        CreateMap<EMRSystem.PrescriptionLabTests.PrescriptionLabTest, PrescriptionLabTestDto>()
    //            .ForMember(dest => dest.ReportTypeName, opt => opt.MapFrom(src => src.LabReportsType.ReportType))
    //            .ReverseMap();
    //    }
    //}
    public class PrescriptionMapProfile : Profile
    {
        public PrescriptionMapProfile()
        {
            // Prescription <-> DTO
            CreateMap<Prescription, PrescriptionDto>()
                .ForMember(dest => dest.LabTestIds, opt => opt.MapFrom(src => src.LabTests.Select(lt => lt.LabReportsTypeId)))
                .ReverseMap();

            CreateMap<CreateUpdatePrescriptionDto, Prescription>()
                .ForMember(dest => dest.Items, opt => opt.Ignore())
                .ForMember(dest => dest.LabTests, opt => opt.Ignore());

            CreateMap<Prescription, CreateUpdatePrescriptionDto>()
                .ForMember(dest => dest.AppointmentId, opt => opt.MapFrom(src => src.Appointment != null ? src.Appointment.Id : (long?)null))
                .ForMember(dest => dest.DoctorId, opt => opt.MapFrom(src => src.Doctor != null ? src.Doctor.Id : (long?)null))
                .ForMember(dest => dest.PatientId, opt => opt.MapFrom(src => src.Patient != null ? src.Patient.Id : (long?)null))
                .ForMember(dest => dest.LabTestIds, opt => opt.MapFrom(src => src.LabTests.Select(lt => lt.LabReportsTypeId)))
                .ForMember(dest => dest.EmergencyProcedures, opt => opt.MapFrom(src => src.SelectedEmergencyProcedureses));

            // Prescription Items
            CreateMap<PrescriptionItem, PrescriptionItemDto>().ReverseMap();
            CreateMap<PrescriptionItem, CreateUpdatePrescriptionItemDto>()
                .ForMember(dest => dest.PrescriptionId, opt => opt.Ignore())
                .ReverseMap();

            // Lab Tests
            CreateMap<LabReports.PrescriptionLabTest, PrescriptionLabTestDto>()
                .ForMember(dest => dest.ReportTypeName, opt => opt.MapFrom(src => src.LabReportsType.ReportType))
                .ReverseMap();

            // 🟢 Missing mappings
            CreateMap<Doctors.ConsultationRequests, CreateUpdateConsultationRequestsDto>().ReverseMap();
            CreateMap<EmergencyProcedure.SelectedEmergencyProcedures, CreateUpdateSelectedEmergencyProceduresDto>().ReverseMap();

            CreateMap<EMRSystem.Prescriptions.Prescription, ViewPrescriptionSummary>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Diagnosis, opt => opt.MapFrom(src => src.Diagnosis))
                .ForMember(dest => dest.IssueDate, opt => opt.MapFrom(src => src.IssueDate))
                .ForMember(dest => dest.PrescribedBy, opt => opt.MapFrom(src => src.Doctor != null ? src.Doctor.FullName : null))
                ;
        }
    }


}
