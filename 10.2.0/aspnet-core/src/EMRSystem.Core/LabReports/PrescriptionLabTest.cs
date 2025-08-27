using Abp.Domain.Entities;
using EMRSystem.Emergency.EmergencyCase;
using EMRSystem.LabMasters;
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

        public bool IsPrescribed { get; set; } // Doctor ne prescribe kiya
        public bool IsFromPackage { get; set; } // Package ka part hai

        // ✅ Link to HealthPackage
        public long? HealthPackageId { get; set; }
        public virtual HealthPackage HealthPackage { get; set; }

        public ICollection<LabReportResultItem> LabReportResultItems { get; set; }
        public bool IsEmergencyPrescription { get; set; }
        public long? EmergencyCaseId { get; set; }
        public virtual EmergencyCase EmergencyCase { get; set; }
    }

    public enum LabTestStatus
    {
        Pending = 0,
        InProgress = 1,
        Completed = 2,
    }

}
