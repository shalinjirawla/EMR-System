using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMRSystem.AppointmentType.Dto
{
    public class AppointmentTypeMapProfile : Profile
    {
        public AppointmentTypeMapProfile()
        {
            CreateMap<AppointmentType, AppointmentTypeDto>();
            CreateMap<CreateUpdateAppointmentTypeDto, AppointmentType>();
        }
    }
}
