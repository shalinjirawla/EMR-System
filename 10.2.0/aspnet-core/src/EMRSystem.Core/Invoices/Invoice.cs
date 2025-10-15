using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using EMRSystem.Appointments;
using EMRSystem.Insurances;
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

        public string InvoiceNo { get; set; }   // Auto generated invoice no
        public InvoiceType InvoiceType { get; set; }

        public DateTime InvoiceDate { get; set; } = DateTime.Now;

        public decimal TotalAmount { get; set; }

        public InvoiceStatus Status { get; set; } = InvoiceStatus.Unpaid;
        public PaymentMethod? PaymentMethod { get; set; }

        public bool IsClaimGenerated { get; set; } = false; // prevent duplicate claim
        public long? InsuranceClaimId { get; set; }                     // claim link
        public ICollection<InsuranceClaim> InsuranceClaims { get; set; }


        public decimal? ApprovedAmount { get; set; }       // insurance approved
        public decimal? CoPayAmount { get; set; }

        public virtual ICollection<InvoiceItem> Items { get; set; } = new List<InvoiceItem>();
    }
    public enum InvoiceStatus
    {
        Unpaid,
        Paid,
        CollectedCoPayAmount
        //PartiallyPaid
    }

    public enum PaymentMethod
    {
        Cash,
        Card
    }
    public enum InvoiceType
    {
        DailyInvoice,
        FullInvoice
    }
}
