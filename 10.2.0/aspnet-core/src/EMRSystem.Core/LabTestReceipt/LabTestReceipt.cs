using Abp.Domain.Entities;
using EMRSystem.Invoices;
using EMRSystem.LabReportsTypes;
using EMRSystem.Patients;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMRSystem.LabTestReceipt
{
    public class LabTestReceipt : Entity<long>, IMustHaveTenant
    {
        public int TenantId { get; set; }

        public long LabReportTypeId { get; set; }

        public long PatientId { get; set; }

        public decimal LabTestFee { get; set; }

        public string ReceiptNumber { get; set; }

        public DateTime PaymentDate { get; set; } = DateTime.Now;

        public PaymentMethod PaymentMethod { get; set; }

        public InvoiceStatus Status { get; set; }
        public virtual EMRSystem.LabReportsTypes.LabReportsType LabReportType { get; set; }
        public virtual Patient Patient { get; set; }
    }
}
