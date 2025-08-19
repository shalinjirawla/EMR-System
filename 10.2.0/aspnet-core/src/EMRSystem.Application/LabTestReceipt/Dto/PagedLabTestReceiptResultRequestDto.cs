using Abp.Application.Services.Dto;
using EMRSystem.Invoices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMRSystem.LabTestReceipt.Dto
{
    public class PagedLabTestReceiptResultRequestDto : PagedResultRequestDto
    {
        public string Keyword { get; set; }
        //public InvoiceStatus? Status { get; set; }
        //public LabTestSource? Source { get; set; }
        //public DateTime? FromDate { get; set; }
        //public DateTime? ToDate { get; set; }
    }
}
