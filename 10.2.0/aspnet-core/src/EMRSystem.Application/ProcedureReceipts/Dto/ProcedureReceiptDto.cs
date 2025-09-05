using Abp.Application.Services.Dto;
using EMRSystem.Invoices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMRSystem.ProcedureReceipts.Dto
{
    public class ProcedureReceiptDto : EntityDto<long>
    {
        public int TenantId { get; set; }
        public long PatientId { get; set; }
        public string PatientName { get; set; } // extra convenience
        public decimal TotalFee { get; set; }
        public string ReceiptNumber { get; set; }
        public DateTime PaymentDate { get; set; }
        public PaymentMethod PaymentMethod { get; set; }
        public InvoiceStatus Status { get; set; }
        public string? PaymentIntentId { get; set; }
    }
}
