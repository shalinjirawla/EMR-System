using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abp.Authorization.Users;
using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using EMRSystem.Authorization.Users;
using EMRSystem.Doctors;
using EMRSystem.Nurses;
using EMRSystem.Patients;
using EMRSystem.Prescriptions;

namespace EMRSystem.Appointments
{
    public class Appointment : Entity<long>, IMustHaveTenant
    {
        public int TenantId { get; set; }
        public DateTime AppointmentDate { get; set; }
        //public TimeSpan StartTime { get; set; } // Use TimeSpan instead of DateTime
        //public TimeSpan EndTime { get; set; }
        public string ReasonForVisit { get; set; }
        public AppointmentStatus Status { get; set; }
        public bool IsFollowUp { get; set; }
        public bool IsPaid { get; set; } = true;
        public long PatientId { get; set; }
        //public PatientType PatientType { get; set; }
        public virtual Patient Patient { get; set; }
        public long DoctorId { get; set; }
        public virtual Doctor Doctor { get; set; }
        //public long? NurseId { get; set; }
        //public virtual Nurse Nurse { get; set; }
        public long? AppointmentTypeId { get; set; }
        public virtual EMRSystem.AppointmentType.AppointmentType AppointmentType { get; set; }
        public ICollection<Prescription> Prescriptions { get; set; }
    }
    public enum AppointmentStatus
    {
        Scheduled,
        Rescheduled,
        CheckedIn,
        Completed,
        Cancelled
    }
    //public enum PatientType
    //{
    //    InPatient,
    //    OutPatient
    //}
}
