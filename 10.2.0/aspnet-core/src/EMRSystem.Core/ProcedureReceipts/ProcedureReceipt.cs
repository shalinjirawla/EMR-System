using Abp.Domain.Entities;
using EMRSystem.EmergencyProcedure;
using EMRSystem.Invoices;
using EMRSystem.LabReports;
using EMRSystem.Patients;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMRSystem.ProcedureReceipts
{
    public class ProcedureReceipt : Entity<long>, IMustHaveTenant
    {
        public int TenantId { get; set; }

        public long PatientId { get; set; }
        public virtual Patient Patient { get; set; }

        public decimal TotalFee { get; set; }

        public string ReceiptNumber { get; set; }

        public DateTime PaymentDate { get; set; } = DateTime.Now;

        public PaymentMethod PaymentMethod { get; set; }

        public InvoiceStatus Status { get; set; }

        public string? PaymentIntentId { get; set; } // optional
        public virtual ICollection<SelectedEmergencyProcedures> SelectedEmergencyProcedures { get; set; }

    }
}
