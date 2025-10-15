using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMRSystem.Insurances.Dto
{
    public class PatientInsuranceMapProfile : Profile
    {
        public PatientInsuranceMapProfile()
        {
            CreateMap<PatientInsurance, PatientInsuranceDto>()
                .ForMember(dest => dest.InsuranceName, opt => opt.Ignore());

            CreateMap<CreateUpdatePatientInsuranceDto, PatientInsurance>();
        }
    }
}
