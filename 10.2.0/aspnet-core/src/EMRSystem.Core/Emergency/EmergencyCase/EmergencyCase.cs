using Abp.Domain.Entities;
using EMRSystem.Admission;
using EMRSystem.Doctors;
using EMRSystem.Nurses;
using EMRSystem.Patients;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMRSystem.Emergency.EmergencyCase
{
    public class EmergencyCase : Entity<long>, IMustHaveTenant
    {
        public int TenantId { get; set; }

        public long? PatientId { get; set; }
        public virtual Patient Patient { get; set; }

        public string EmergencyNumber { get; set; }
        public DateTime ArrivalTime { get; set; } = DateTime.Now;
        public DateTime? DischargeTime { get; set; } // Important for analytics

        public ModeOfArrival ModeOfArrival { get; set; }
        public EmergencySeverity Severity { get; set; }
        public EmergencyStatus Status { get; set; } = EmergencyStatus.PendingTriage;

        public long? DoctorId { get; set; }
        public virtual Doctor Doctor { get; set; }
        public long? NurseId { get; set; }
        public virtual Nurse Nurse { get; set; }
        public long? AdmissionsId { get; set; }
        public virtual EMRSystem.Admission.Admission Admissions { get; set; }
        public virtual ICollection<Triage.Triage> Triages { get; set; }
    }

    public enum ModeOfArrival
    {
        WalkIn,
        Ambulance,
        Police ,
        Unknown
    }

    public enum EmergencySeverity
    {
        Critical,
        Serious,
        Stable
    }

    public enum EmergencyStatus
    {
        PendingTriage, // Just arrived, not yet triaged
        Waiting,       // Triaged, waiting for treatment
        InTreatment,   // Currently with a doctor/nurse
        AdmissionPending, // Decision made to admit, waiting for bed
        Admitted,      // Transferred to a ward
        Discharged,    // Sent home
    }
}