using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMRSystem.Deposit.Dto
{
    public class DepositApplicationAutoMapperProfile : Profile
    {
        public DepositApplicationAutoMapperProfile()
        {
            CreateMap<PatientDeposit, PatientDepositDto>()
                .ForMember(d => d.PatientName, opt => opt.MapFrom(s => s.Patient != null ? s.Patient.FullName : ""));
            CreateMap<CreateUpdatePatientDepositDto, PatientDeposit>();

            CreateMap<DepositTransaction, DepositTransactionDto>();
            CreateMap<CreateUpdateDepositTransactionDto, DepositTransaction>();
        }
    }
}
