using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using EMRSystem.Authorization.Users;
using EMRSystem.Doctors;
using EMRSystem.Prescriptions;
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
        public long? AssignedNurseId { get; set; }
        public bool IsAdmitted { get; set; }
        public DateTime? AdmissionDate { get; set; }
        public DateTime? DischargeDate { get; set; }
        public string? InsuranceProvider { get; set; }
        public string? InsurancePolicyNumber { get; set; }
        public long AbpUserId { get; set; }
        public virtual User AbpUser { get; set; }
        public long? AssignedDoctorId { get; set; }
        public virtual Doctor Doctors { get; set; }
        public ICollection<Prescription> Prescriptions { get; set; }
        public ICollection<Vital> Vitals { get; set; }
    }
}
