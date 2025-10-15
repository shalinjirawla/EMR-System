using Abp.Application.Services.Dto;
using EMRSystem.Insurances.Dto;
using EMRSystem.Invoices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMRSystem.Invoice.Dto
{
    public class InvoiceDto : EntityDto<long>
    {
        public int TenantId { get; set; }
        public long PatientId { get; set; }
        public string PatientName { get; set; }
        public string InvoiceNo { get; set; }
        public InvoiceType InvoiceType { get; set; }
        public DateTime InvoiceDate { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal? ApprovedAmount { get; set; }   // ✅ add this
        public decimal? CoPayAmount { get; set; }
        public InvoiceStatus Status { get; set; }
        public PaymentMethod? PaymentMethod { get; set; }
        public List<InsuranceClaimDto> Claims { get; set; }

        public List<InvoiceItemDto> Items { get; set; }
    }
}
