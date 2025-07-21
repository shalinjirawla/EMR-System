using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMRSystem.Deposit.Dto
{
    public class DepositMapProfile : Profile
    {
        public DepositMapProfile()
        {
            CreateMap<Deposit, DepositDto>()
                    .ForMember(dest => dest.PatientName,
                        opt => opt.MapFrom(src => src.Patient != null && src.Patient != null
                            ? src.Patient.FullName
                            : string.Empty));
            CreateMap<CreateUpdateDepositDto, Deposit>();
        }
    }
}
