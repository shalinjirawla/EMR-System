using AutoMapper;
using EMRSystem.Authorization.Roles;
using EMRSystem.Billings;
using EMRSystem.BillingStaff.Dto;
using EMRSystem.Prescriptions.Dto;
using EMRSystem.Prescriptions;
using EMRSystem.Roles.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMRSystem.Appointments.Dto
{
    class AppointmentMapProfile : Profile
    {
        public AppointmentMapProfile()
        {
            CreateMap<Appointment, AppointmentDto>().ReverseMap();
            CreateMap<Appointment, CreateUpdateAppointmentDto>()
              .ForMember(dest => dest.NurseId, opt => opt.MapFrom(src => src.Nurse.Id))
             .ForMember(dest => dest.DoctorId, opt => opt.MapFrom(src => src.Doctor.Id))
             .ForMember(dest => dest.PatientId, opt => opt.MapFrom(src => src.Patient.Id))
            .ReverseMap();
        }
    }
}
