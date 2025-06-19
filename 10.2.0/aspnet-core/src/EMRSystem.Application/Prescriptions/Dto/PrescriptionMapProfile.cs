using AutoMapper;
using EMRSystem.LabReports;
using EMRSystem.LabTechnician.Dto;
using EMRSystem.Vitals.Dto;
using EMRSystem.Vitals;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMRSystem.Prescriptions.Dto
{
    class PrescriptionMapProfile : Profile
    {
        public PrescriptionMapProfile()
        {
            CreateMap<Prescription, PrescriptionDto>().ReverseMap();
            CreateMap<CreateUpdatePrescriptionDto, Prescription>();
            CreateMap<Prescription, CreateUpdatePrescriptionDto>()
               .ForMember(dest => dest.AppointmentId, opt => opt.MapFrom(src => src.Appointment.Id))
               .ForMember(dest => dest.DoctorId, opt => opt.MapFrom(src => src.Doctor.Id))
               .ForMember(dest => dest.PatientId, opt => opt.MapFrom(src => src.Patient.Id));

            CreateMap<PrescriptionItem, PrescriptionItemDto>().ReverseMap();
            CreateMap<PrescriptionItem, CreateUpdatePrescriptionItemDto>().ReverseMap();
        }
    }
}
