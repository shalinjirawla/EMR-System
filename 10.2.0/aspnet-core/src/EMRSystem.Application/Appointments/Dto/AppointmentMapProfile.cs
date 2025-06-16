using AutoMapper;
using EMRSystem.Authorization.Roles;
using EMRSystem.Billings;
using EMRSystem.BillingStaff.Dto;
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
            CreateMap<Appointment, AppointmentDto>()
    .ForMember(x => x.PatientName, opt => opt.MapFrom(x => x.Patient != null ? x.Patient.FullName : ""))
    .ForMember(x => x.DoctorName, opt => opt.MapFrom(x => x.Doctor != null ? x.Doctor.FullName : ""))
    .ReverseMap();

            CreateMap<Appointment, CreateUpdateAppointmentDto>().ReverseMap();
        }
    }
}
