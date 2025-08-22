using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using EMRSystem.Appointments;
using EMRSystem.Authorization.Users;
using EMRSystem.Deposit;
using EMRSystem.Doctors;
using EMRSystem.Emergency.EmergencyCase;
using EMRSystem.Invoices;
using EMRSystem.LabReports;
using EMRSystem.Nurses;
using EMRSystem.Prescriptions;
using EMRSystem.Visits;
using EMRSystem.Vitals;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMRSystem.Patients
{
    public class Patient : Entity<long>, IMustHaveTenant
    {
        public long AbpUserId { get; set; }
        public int TenantId { get; set; }
        public decimal CurrentBalance { get; set; } = 0;
        public bool IsAdmitted { get; set; } = false;
        public string FullName { get; set; }
        public string Gender { get; set; }
        public string Address { get; set; }
        public string BloodGroup { get; set; }
        public string EmergencyContactName { get; set; }
        public string EmergencyContactNumber { get; set; }
        public DateTime DateOfBirth { get; set; }
        public DateTime? LastBillingDate { get; set; }
        public virtual User AbpUser { get; set; }
        public ICollection<Prescription> Prescriptions { get; set; }
        public virtual ICollection<EMRSystem.Admission.Admission> Admissions { get; set; }
        public virtual ICollection<PatientDeposit> PatientDeposits { get; set; }
        public ICollection<Vital> Vitals { get; set; }
        public virtual ICollection<Appointment> Appointments { get; set; }
        public ICollection<Visit> Visit { get; set; }
        public virtual ICollection<MedicineOrder.MedicineOrder> MedicineOrders { get; set; }
        public virtual ICollection<PrescriptionLabTest> PrescriptionLabTests { get; set; }
        public virtual ICollection<EMRSystem.IpdChargeEntry.IpdChargeEntry> IpdChargeEntries { get; set; }
        public virtual ICollection<EMRSystem.LabTestReceipt.LabTestReceipt> LabTestReceipts { get; set; }
        public virtual ICollection<EmergencyCase> EmergencyCases { get; set; }


        //public DateTime? AdmissionDate { get; set; }
        //public BillingMethod BillingMethod { get; set; }
        //public PaymentMethod? PaymentMethod { get; set; }
        //public long? RoomId { get; set; }
        //public virtual EMRSystem.Room.Room Room { get; set; }

        //public long? DepositAmount { get; set; }
        //public DateTime? DischargeDate { get; set; }
        //public string? InsuranceProvider { get; set; }
        //public string? InsurancePolicyNumber { get; set; }
        //public long? AssignedNurseId { get; set; }
        //public long? AssignedDoctorId { get; set; }
        //public virtual Nurse Nurses { get; set; }
        //public virtual Doctor Doctors { get; set; }

    }
    public enum BillingMethod
    {
        InsuranceOnly,
        SelfPay,
        InsuranceSelfPay
    }
}
