using Abp.Domain.Entities;
using EMRSystem.LabReportsTypes;
using EMRSystem.LabTestReceipt;
using EMRSystem.Patients;
using EMRSystem.Prescriptions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMRSystem.LabReports
{
    public class PrescriptionLabTest : Entity<long>, IMustHaveTenant
    {
        public int TenantId { get; set; }
        public long? PrescriptionId { get; set; }
        public virtual Prescription Prescription { get; set; }
        public long? PatientId { get; set; }
        public virtual Patient Patient { get; set; }
        public long LabReportsTypeId { get; set; }
        public virtual LabReportsType LabReportsType { get; set; }
        public LabTestStatus TestStatus { get; set; }
        public bool IsPaid { get; set; }=false;
        public DateTime? CreatedDate { get; set; } = DateTime.Now;
        public long? LabTestReceiptId { get; set; }
        public virtual EMRSystem.LabTestReceipt.LabTestReceipt LabTestReceipt { get; set; }
        public ICollection<LabReportResultItem> LabReportResultItems { get; set; }
    }

    public enum LabTestStatus
    {
        Pending = 0,
        InProgress = 1,
        Completed = 2,
    }

}
