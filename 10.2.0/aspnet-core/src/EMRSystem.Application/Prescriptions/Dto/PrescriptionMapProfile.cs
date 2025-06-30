using AutoMapper;
using EMRSystem.LabTechnician.Dto;
using EMRSystem.PrescriptionLabTest.Dto;
using EMRSystem.Vitals;
using EMRSystem.Vitals.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EMRSystem.Prescriptions.Dto;
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
            // Prescription Mappings
            CreateMap<Prescription, PrescriptionDto>()
                .ForMember(dest => dest.LabTestIds, opt => opt.MapFrom(src => src.LabTests.Select(lt => lt.LabReportsTypeId)))
                .ReverseMap();

            CreateMap<CreateUpdatePrescriptionDto, Prescription>()
                .ForMember(dest => dest.Items, opt => opt.Ignore())  // Handle separately
                .ForMember(dest => dest.LabTests, opt => opt.Ignore()); // Handle separately

            CreateMap<Prescription, CreateUpdatePrescriptionDto>()
                .ForMember(dest => dest.AppointmentId, opt => opt.MapFrom(src => src.Appointment.Id))
                .ForMember(dest => dest.DoctorId, opt => opt.MapFrom(src => src.Doctor.Id))
                .ForMember(dest => dest.PatientId, opt => opt.MapFrom(src => src.Patient.Id))
                .ForMember(dest => dest.LabTestIds, opt => opt.MapFrom(src => src.LabTests.Select(lt => lt.LabReportsTypeId)));

            // Prescription Items
            CreateMap<PrescriptionItem, PrescriptionItemDto>().ReverseMap();
            CreateMap<PrescriptionItem, CreateUpdatePrescriptionItemDto>()
                .ForMember(dest => dest.PrescriptionId, opt => opt.Ignore())
                .ReverseMap();

            // Lab Tests
            CreateMap<LabReports.PrescriptionLabTest, PrescriptionLabTestDto>()
                .ForMember(dest => dest.ReportTypeName, opt => opt.MapFrom(src => src.LabReportsType.ReportType))
                .ReverseMap();
        }
    }

}
