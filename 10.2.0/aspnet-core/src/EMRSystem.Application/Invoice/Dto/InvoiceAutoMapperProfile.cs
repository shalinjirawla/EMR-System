using AutoMapper;
using EMRSystem.Invoices;
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
            // Invoice mappings
            CreateMap<CreateUpdateInvoiceDto, EMRSystem.Invoices.Invoice>();

            CreateMap<EMRSystem.Invoices.Invoice, InvoiceDto>()
                .ForMember(dest => dest.PatientName, opt => opt.MapFrom(src => src.Patient.FullName))
                .ForMember(dest => dest.AppointmentDate, opt => opt.MapFrom(src =>
                    src.Appointment.AppointmentDate));

            // InvoiceItem mappings
            CreateMap<CreateUpdateInvoiceItemDto, InvoiceItem>();

            CreateMap<InvoiceItem, InvoiceItemDto>();
        }
    }
}
