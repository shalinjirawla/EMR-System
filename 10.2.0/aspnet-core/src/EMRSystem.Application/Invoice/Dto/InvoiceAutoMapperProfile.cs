using AutoMapper;
using EMRSystem.Insurances;
using EMRSystem.Insurances.Dto;
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
                .ForMember(dest => dest.PatientDOB, opt => opt.MapFrom(src => src.Patient.DateOfBirth))
                .ForMember(dest => dest.PatientBloodGroup, opt => opt.MapFrom(src => src.Patient.BloodGroup))
                .ForMember(dest => dest.PatientGender, opt => opt.MapFrom(src => src.Patient.Gender))
                .ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.Items))
                .ForMember(dest => dest.Claims, opt => opt.MapFrom(src => src.InsuranceClaims));

            CreateMap<InsuranceClaim, InsuranceClaimDto>()
                .ForMember(dest => dest.PatientName, opt => opt.MapFrom(src => src.Invoice.Patient.FullName))
                .ForMember(dest => dest.InvoiceNo, opt => opt.MapFrom(src => src.Invoice.InvoiceNo))
                .ForMember(dest => dest.InsuranceName, opt => opt.MapFrom(src => src.PatientInsurance.InsuranceMaster.InsuranceName));

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
