using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMRSystem.ProcedureReceipts.Dto
{
    public class ProcedureReceiptMapProfile : Profile
    {
        public ProcedureReceiptMapProfile()
        {
            CreateMap<ProcedureReceipt, ProcedureReceiptDto>()
                .ForMember(dest => dest.PatientName, opt => opt.MapFrom(src => src.Patient != null ? src.Patient.FullName : string.Empty));

            CreateMap<CreateUpdateProcedureReceiptDto, ProcedureReceipt>();
        }
    }
}
