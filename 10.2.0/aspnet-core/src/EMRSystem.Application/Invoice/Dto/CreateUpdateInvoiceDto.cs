using Abp.Application.Services.Dto;
using Abp.Domain.Entities;
using EMRSystem.Invoices;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMRSystem.Invoice.Dto
{
    public class CreateUpdateInvoiceDto : EntityDto<long>
    {
        public int TenantId { get; set; }
        public long PatientId { get; set; }
        public InvoiceType InvoiceType { get; set; }
        public DateTime InvoiceDate { get; set; } = DateTime.Now;
        public decimal TotalAmount { get; set; }
        public InvoiceStatus Status { get; set; }
        public PaymentMethod? PaymentMethod { get; set; }
        public List<CreateUpdateInvoiceItemDto> Items { get; set; } = new();
    }
}
