using AutoMapper;
using EMRSystem.Billings;
using EMRSystem.BillingStaff.Dto;
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
            CreateMap<Appointment, CreateUpdateAppointmentDto>().ReverseMap();
        }
    }
}
