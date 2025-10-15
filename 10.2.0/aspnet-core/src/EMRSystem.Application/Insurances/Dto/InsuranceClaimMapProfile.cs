using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMRSystem.Insurances.Dto
{
    public class InsuranceClaimMapProfile : Profile
    {
        public InsuranceClaimMapProfile()
        {
            CreateMap<InsuranceClaim, InsuranceClaimDto>()
            .ForMember(dest => dest.InvoiceNo, opt => opt.MapFrom(src => src.Invoice.InvoiceNo))
            .ForMember(dest => dest.PatientName, opt => opt.MapFrom(src => src.Invoice.Patient.FullName))
            .ForMember(dest => dest.InsuranceName, opt => opt.MapFrom(src => src.PatientInsurance.InsuranceMaster.InsuranceName));

            CreateMap<CreateUpdateInsuranceClaimDto, InsuranceClaim>();
        }
    }
}
