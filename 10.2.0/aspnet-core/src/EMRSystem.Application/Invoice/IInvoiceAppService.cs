using Abp.Application.Services;
using Abp.Application.Services.Dto;
using EMRSystem.Doctor.Dto;
using EMRSystem.Invoice.Dto;
using EMRSystem.Invoices;
using EMRSystem.IpdChargeEntry.Dto;
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
        Task<List<IpdChargeEntryDto>> GetChargesByPatientAsync(long patientId, InvoiceType invoiceType);
        Task<List<InvoiceDto>> GetInvoicesByPatientID(long patientID);
    }

}
