using Abp.Domain.Entities;
using EMRSystem.Patients;
using EMRSystem.Prescriptions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMRSystem.IpdChargeEntry
{
    public class IpdChargeEntry : Entity<long>, IMustHaveTenant
    {
        public int TenantId { get; set; }
        public long AdmissionId { get; set; }
        public long PatientId { get; set; }
        public ChargeType ChargeType { get; set; }
        public string Description { get; set; } 
        public decimal Amount { get; set; }
        public DateTime EntryDate { get; set; } = DateTime.Now;
        public bool IsProcessed { get; set; } = false;
        public long? ReferenceId { get; set; }
        public int Quantity { get; set; } = 1;

        public virtual Admission.Admission Admission { get; set; }
        public virtual Patient Patient { get; set; }
        public long? PrescriptionId { get; set; }
        public virtual Prescription Prescriptions { get; set; }
    }

    public enum ChargeType
    {
        Appointment,
        LabTest,
        Medicine,
        ConsultationFee,
        Procedure,
        Room,
        Other
    }
}
