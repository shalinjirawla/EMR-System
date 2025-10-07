using Abp.Domain.Entities;
using System;

namespace EMRSystem.PatientDischarge
{
    public class PatientDischarge : Entity<long>, IMustHaveTenant
    {
        public int TenantId { get; set; }
        public long AdmissionId { get; set; }
        public long PatientId { get; set; }
        public long? DoctorId { get; set; }

        public DateTime? DischargeDate { get; set; }
        public string DischargeSummary { get; set; }
        public DischargeStatus DischargeStatus { get; set; }

        // ✅ Newly Added Fields
        public string? ProvisionalDiagnosis { get; set; }
        public string? FinalDiagnosis { get; set; }
        public string? InvestigationSummary { get; set; }
        public string? ConditionAtDischarge { get; set; }
        public string? DietAdvice { get; set; }
        public string? Activity { get; set; }
        public DateTime? FollowUpDate { get; set; }
        public long? FollowUpDoctorId { get; set; }

        // ✅ Navigation Properties
        public virtual EMRSystem.Admission.Admission Admission { get; set; }
        public virtual EMRSystem.Patients.Patient Patient { get; set; }
        public virtual EMRSystem.Doctors.Doctor Doctor { get; set; }
        public virtual EMRSystem.Doctors.Doctor FollowUpDoctor { get; set; } // for follow-up mapping

        //public virtual EMRSystem.Pharmacists.Pharmacist PharmacistStaff { get; set; }
        //public virtual EMRSystem.Billings.Bill BillingStaff { get; set; }
    }
    public enum DischargeStatus
    {
        Pending = 0,
        Initiated = 1,
        SentToDoctor = 2,
        DoctorVerified = 3,
        SentToLabTechnician = 4,
        LabTechnicianCompleted = 5,
        SentToBilling = 6,
        BillingCompleted = 7,
        FinalApproval = 8,
        Discharged = 9,
    }
}
