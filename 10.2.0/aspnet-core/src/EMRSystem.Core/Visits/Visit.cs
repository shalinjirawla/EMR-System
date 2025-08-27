using Abp.Domain.Entities;
using EMRSystem.Departments;
using EMRSystem.Doctors;
using EMRSystem.Nurses;
using EMRSystem.Patients;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMRSystem.Visits
{
    public class Visit : Entity<long>, IMustHaveTenant
    {
        public int TenantId { get; set; }
        public long PatientId { get; set; }
        public virtual Patient Patient { get; set; }
        //public long DepartmentId { get; set; }
        //public virtual Department Department { get; set; }
        public long NurseId { get; set; }
        public virtual Nurse Nurse { get; set; }
        public long DoctorId { get; set; }
        public virtual Doctor Doctor { get; set; }
        public DateTime DateOfVisit { get; set; }
        public TimeSpan TimeOfVisit { get; set; }
        public string ReasonForVisit { get; set; }
        public PaymentMode PaymentMode { get; set; }
        public decimal? ConsultationFee { get; set; }
    }
    public enum PaymentMode
    {
        Cash,
        Card,
        Insurance,
    }
}
