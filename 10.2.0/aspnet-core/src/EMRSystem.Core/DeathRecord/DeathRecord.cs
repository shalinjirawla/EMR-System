using Abp.Domain.Entities;
using EMRSystem.Doctors;
using EMRSystem.Nurses;
using EMRSystem.Patients;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMRSystem.DeathRecord
{
    public class DeathRecord : Entity<long>, IMustHaveTenant
    {
        public int TenantId { get; set; }
        public long PatientId { get; set; }
        public DateTime DeathDate { get; set; }
        public DateTime DeathTime { get; set; }
        public long? DoctorId { get; set; }
        public long? NurseId { get; set; }
        public string? CauseOfDeath { get; set; }
        public bool IsPostMortemDone { get; set; }
        public string? Notes { get; set; }
        public virtual Patient Patient { get; set; }
        public virtual Doctor Doctor { get; set; }
        public virtual Nurse Nurse { get; set; }
    }
}
