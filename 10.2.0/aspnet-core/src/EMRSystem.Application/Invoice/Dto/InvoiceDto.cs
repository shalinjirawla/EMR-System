using Abp.Application.Services.Dto;
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
        public long AppointmentId { get; set; }
        public DateTime AppointmentDate { get; set; }
        public DateTime InvoiceDate { get; set; }
        public DateTime DueDate { get; set; }
        public decimal SubTotal { get; set; }
        public decimal GstAmount { get; set; }
        public decimal TotalAmount { get; set; }
        public InvoiceStatus Status { get; set; }
        public PaymentMethod? PaymentMethod { get; set; }
        //public List<InvoiceItemDto> Items { get; set; } = new();
    }
}
