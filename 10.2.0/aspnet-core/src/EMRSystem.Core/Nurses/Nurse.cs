using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using EMRSystem.Appointments;
using EMRSystem.Authorization.Users;
using EMRSystem.Patients;
using EMRSystem.Visits;
using EMRSystem.Vitals;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMRSystem.Nurses
{
    public class Nurse : Entity<long>, IMustHaveTenant
    {
        public int TenantId { get; set; }
        public string FullName { get; set; }
        public string Gender { get; set; }
        public string ShiftTiming { get; set; }
        public string Department { get; set; }
        public string Qualification { get; set; }
        public int YearsOfExperience { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public long AbpUserId { get; set; }
        public virtual User AbpUser { get; set; }
        public ICollection<Vital> Vitals { get; set; }
       // public ICollection<Patient> Patients { get; set; }
        public virtual ICollection<Appointment> Appointments { get; set; }
        public ICollection<Visit> Visits { get; set; }
        public virtual ICollection<MedicineOrder.MedicineOrder> MedicineOrders { get; set; }
        public virtual ICollection<EMRSystem.Admission.Admission> Admissions { get; set; }



    }
}
