using AutoMapper;
using EMRSystem.DoctorMaster.Dto;
using EMRSystem.Emergency.EmergencyCase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMRSystem.EmergencyMaster.Dto
{
    public class EmergencyMasterMapProfile : Profile
    {
        public EmergencyMasterMapProfile()
        {
            CreateMap<EMRSystem.Emergency.EmergencyMaster.EmergencyMaster, EmergencyMasterDto>().ReverseMap();
            CreateMap<EMRSystem.Emergency.EmergencyMaster.EmergencyMaster, CreateUpdateEmergencyMasterDto>().ReverseMap();
        }
    }
}
