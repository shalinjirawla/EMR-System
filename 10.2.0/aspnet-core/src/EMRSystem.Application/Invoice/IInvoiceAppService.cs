using Abp.Application.Services;
using Abp.Application.Services.Dto;
using EMRSystem.Doctor.Dto;
using EMRSystem.Invoice.Dto;
using EMRSystem.LabReport.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMRSystem.Invoice
{
    public interface IInvoiceAppService : IAsyncCrudAppService<
   InvoiceDto, long, PagedInvoiceResultRequestDto, CreateUpdateInvoiceDto, CreateUpdateInvoiceDto>
    {
        Task<InvoiceDetailsDto> GetInvoiceDetailsByAppointmentIdUsingSp(long appointmentId);
        Task MarkAsPaid(long invoiceId);
        public  Task<string> CreateStripeCheckoutSession(long invoiceId, decimal amount, string successUrl, string cancelUrl);
    }

}
