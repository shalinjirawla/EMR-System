using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMRSystem.EmergencyProcedure.Dto
{
    public class EmergencyProcedureApplicationAutoMapperProfile : Profile
    {
        public EmergencyProcedureApplicationAutoMapperProfile()
        {
            CreateMap<EmergencyProcedure, EmergencyProcedureDto>();
            CreateMap<CreateUpdateEmergencyProcedureDto, EmergencyProcedure>();
        }
    }
}
