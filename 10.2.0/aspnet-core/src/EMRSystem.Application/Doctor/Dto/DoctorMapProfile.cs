using EMRSystem.Patients.Dto;
using EMRSystem.Patients;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;

namespace EMRSystem.Doctor.Dto
{
    public class DoctorMapProfile : Profile
    {
        public DoctorMapProfile()
        {
            CreateMap<EMRSystem.Doctors.Doctor, DoctorDto>()
                 .ForMember(dest => dest.Department, opt => opt.MapFrom(src => src.Department)); // Map Department
            CreateMap<EMRSystem.Doctors.Doctor, CreateUpdateDoctorDto>().ReverseMap();

            CreateMap<EMRSystem.Doctors.ConsultationRequests, ConsultationRequestsDto>()
            .ForMember(dest => dest.PatientName,
                        opt => opt.MapFrom(src => src.Prescriptions.Patient != null ? src.Prescriptions.Patient.FullName : null))
            .ForMember(dest => dest.RequestingDoctorName,
                        opt => opt.MapFrom(src => src.RequestingDoctor != null ? src.RequestingDoctor.FullName : null))
            .ForMember(dest => dest.RequestedSpecialistName,
                        opt => opt.MapFrom(src => src.RequestedSpecialist != null ? src.RequestedSpecialist.FullName : null))
            .ForMember(dest => dest.PrescriptionId, opt => opt.MapFrom(src => src.Prescriptions.Id))
            .ForMember(dest => dest.RequestingDoctorId, opt => opt.MapFrom(src => src.RequestingDoctor.Id))
            .ForMember(dest => dest.RequestedSpecialistId, opt => opt.MapFrom(src => src.RequestedSpecialist.Id))
             .ReverseMap();

            CreateMap<EMRSystem.Doctors.ConsultationRequests, CreateUpdateConsultationRequestsDto>().ReverseMap();
        }
    }
}
