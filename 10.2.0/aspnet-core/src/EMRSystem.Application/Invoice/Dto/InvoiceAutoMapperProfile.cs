using AutoMapper;
using EMRSystem.Invoices;
using EMRSystem.IpdChargeEntry.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMRSystem.Invoice.Dto
{
    public class InvoiceAutoMapperProfile : Profile
    {
        public InvoiceAutoMapperProfile()
        {
            // Invoice -> InvoiceDto
            CreateMap<EMRSystem.Invoices.Invoice, InvoiceDto>()
               .ForMember(dest => dest.PatientName, opt => opt.MapFrom(src => src.Patient.FullName))
               //.ForMember(dest => dest.InvoiceType, opt => opt.MapFrom(src => src.InvoiceType.ToString()))
               .ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.Items));

            // InvoiceItem -> InvoiceItemDto
            CreateMap<InvoiceItem, InvoiceItemDto>();

            // CreateUpdateInvoiceDto -> Invoice
            CreateMap<CreateUpdateInvoiceDto, EMRSystem.Invoices.Invoice>()
                .ForMember(dest => dest.InvoiceNo, opt => opt.Ignore())
                .ForMember(dest => dest.Items, opt => opt.Ignore());

            // CreateUpdateInvoiceItemDto -> InvoiceItem
            CreateMap<CreateUpdateInvoiceItemDto, InvoiceItem>();
            CreateMap<InvoiceItem, InvoiceItemDto>();

            // IpdChargeEntry -> IpdChargeEntryDto
            CreateMap<EMRSystem.IpdChargeEntry.IpdChargeEntry, IpdChargeEntryDto>()
                .ForMember(dest => dest.PatientName, opt => opt.MapFrom(src => src.Patient.FullName))
                .ForMember(dest => dest.ChargeType, opt => opt.MapFrom(src => src.ChargeType.ToString()));
        }
    }
}
