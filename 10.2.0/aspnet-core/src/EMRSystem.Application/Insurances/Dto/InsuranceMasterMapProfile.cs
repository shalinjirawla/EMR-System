using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMRSystem.Insurances.Dto
{
    public class InsuranceMasterMapProfile : Profile
    {
        public InsuranceMasterMapProfile()
        {
            CreateMap<InsuranceMaster, InsuranceMasterDto>();
            CreateMap<CreateUpdateInsuranceMasterDto, InsuranceMaster>();
        }
    }
}
