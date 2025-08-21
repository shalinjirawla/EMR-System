using Abp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMRSystem.Emergency.Triage
{
    public class Triage : Entity<long>, IMustHaveTenant
    {
        public int TenantId { get; set; }

        public long EmergencyCaseId { get; set; }
        public virtual EmergencyCase.EmergencyCase EmergencyCase { get; set; }

        //public string TriageLevel { get; set; } // Red, Yellow, Green
        public string Notes { get; set; }

        public decimal? Temperature { get; set; }
        public int? Pulse { get; set; }
        public int? RespiratoryRate { get; set; }
        public int? BloodPressureSystolic { get; set; }
        public int? BloodPressureDiastolic { get; set; }

        public DateTime AssessmentTime { get; set; } = DateTime.Now;
    }
}
