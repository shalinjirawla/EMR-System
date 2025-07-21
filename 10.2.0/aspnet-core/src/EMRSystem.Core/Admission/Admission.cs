using Abp.Domain.Entities;
using EMRSystem.Doctors;
using EMRSystem.Nurses;
using EMRSystem.Patients;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMRSystem.Admission
{
    public class Admission : Entity<long>, IMustHaveTenant
    {
        public int TenantId { get; set; }
        public long PatientId { get; set; }
        public DateTime AdmissionDateTime { get; set; }
        public long DoctorId { get; set; }
        public long? NurseId { get; set; }
        public long RoomId { get; set; }
        public AdmissionType AdmissionType { get; set; }

        // Navigation
        public virtual EMRSystem.Patients.Patient Patient { get; set; }
        public virtual EMRSystem.Doctors.Doctor Doctor { get; set; }
        public virtual EMRSystem.Nurses.Nurse Nurse { get; set; }
        public virtual EMRSystem.Room.Room Room { get; set; }
        //public virtual ICollection<EMRSystem.Deposit.Deposit> Deposits { get; set; } = new List<EMRSystem.Deposit.Deposit>();
    }
    public enum AdmissionType
    {
        InPatient = 0,
        DayCare = 1,
        Emergency = 2
    }
}
