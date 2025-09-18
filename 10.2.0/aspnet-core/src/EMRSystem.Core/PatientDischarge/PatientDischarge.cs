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
        //public long? BillingStaffId { get; set; }
        //public long? PharmacyStaffId { get; set; }


        public DateTime? DischargeDate { get; set; }
        public string DischargeSummary { get; set; }
        public DischargeStatus DischargeStatus { get; set; }


        public virtual EMRSystem.Admission.Admission Admission { get; set; }
        public virtual EMRSystem.Patients.Patient Patient { get; set; }
        public virtual EMRSystem.Doctors.Doctor Doctor { get; set; }
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
