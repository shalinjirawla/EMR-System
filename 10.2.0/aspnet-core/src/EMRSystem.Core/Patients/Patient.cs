using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using EMRSystem.Appointments;
using EMRSystem.Authorization.Users;
using EMRSystem.Doctors;
using EMRSystem.Invoices;
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
        public int TenantId { get; set; }
        public string FullName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Gender { get; set; }
        public string Address { get; set; }
        public string BloodGroup { get; set; }
        public string EmergencyContactName { get; set; }
        public string EmergencyContactNumber { get; set; }
        public bool IsAdmitted { get; set; } = false;
        //public DateTime? AdmissionDate { get; set; }
        //public BillingMethod BillingMethod { get; set; }
        //public PaymentMethod? PaymentMethod { get; set; }
        //public long? RoomId { get; set; }
        //public virtual EMRSystem.Room.Room Room { get; set; }

        //public long? DepositAmount { get; set; }
        //public DateTime? DischargeDate { get; set; }
        //public string? InsuranceProvider { get; set; }
        //public string? InsurancePolicyNumber { get; set; }

        public long AbpUserId { get; set; }
        //public long? AssignedNurseId { get; set; }
        //public long? AssignedDoctorId { get; set; }
        public virtual User AbpUser { get; set; }
        //public virtual Nurse Nurses { get; set; }
        //public virtual Doctor Doctors { get; set; }
        public ICollection<Prescription> Prescriptions { get; set; }
        public virtual ICollection<EMRSystem.Admission.Admission> Admissions { get; set; }
        public virtual ICollection<EMRSystem.Deposit.Deposit> Deposits { get; set; }


        public ICollection<Vital> Vitals { get; set; }
        public virtual ICollection<Appointment> Appointments { get; set; }
        public ICollection<Visit> Visit { get; set; }
        public virtual ICollection<MedicineOrder.MedicineOrder> MedicineOrders { get; set; }

    }
    public enum BillingMethod
    {
        InsuranceOnly,
        SelfPay,
        InsuranceSelfPay
    }
}
