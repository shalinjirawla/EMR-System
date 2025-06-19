using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using EMRSystem.Nurses;
using EMRSystem.Patients;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMRSystem.Vitals
{
    public class Vital : Entity<long>, IMustHaveTenant
    {
        public int TenantId { get; set; }
        public DateTime? DateRecorded { get; set; } = DateTime.Now;
        public string BloodPressure { get; set; } 
        public string HeartRate { get; set; }
        public int RespirationRate { get; set; }
        public decimal Temperature { get; set; }
        public int OxygenSaturation { get; set; }
        public string Height { get; set; }
        public string Weight { get; set; }
        public string BMI { get; set; }
        public string Notes { get; set; }
        public long PatientId { get; set; }
        public virtual Patient Patient { get; set; }
        public long NurseId { get; set; }
        public virtual Nurse Nurse { get; set; }
    }
}
