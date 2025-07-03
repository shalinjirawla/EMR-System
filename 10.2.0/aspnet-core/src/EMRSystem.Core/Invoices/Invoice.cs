using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using EMRSystem.Appointments;
using EMRSystem.Patients;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMRSystem.Invoices
{
    public class Invoice : Entity<long>, IMustHaveTenant
    {
        public int TenantId { get; set; }
        public long PatientId { get; set; }
        public virtual Patient Patient { get; set; }
        public long AppointmentId { get; set; }
        public virtual Appointment Appointment { get; set; }
        public DateTime InvoiceDate { get; set; } = DateTime.Now;
        public DateTime DueDate { get; set; } = DateTime.Now.AddDays(15);
        public decimal SubTotal { get; set; }
        public decimal GstAmount { get; set; }
        public decimal TotalAmount { get; set; }
        public InvoiceStatus Status { get; set; } = InvoiceStatus.Unpaid;
        public PaymentMethod? PaymentMethod { get; set; }

        public virtual ICollection<InvoiceItem> Items { get; set; } = new List<InvoiceItem>();
    }
    public enum InvoiceStatus
    {
        Unpaid,
        Paid,
        PartialPaid
    }

    public enum PaymentMethod
    {
        Cash,
        Card
    }
}
