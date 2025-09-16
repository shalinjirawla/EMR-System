using AutoMapper;
using EMRSystem.MedicineForms.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMRSystem.StrengthUnitMaster.Dto
{
    public class StrengthUnitMasterMapProfile : Profile
    {
        public StrengthUnitMasterMapProfile()
        {

            CreateMap<StrengthUnitMaster, StrengthUnitMasterDto>();
            CreateMap<CreateUpdateStrengthUnitMasterDto, StrengthUnitMaster>();
        }
    }
}
