using Abp.Domain.Entities;
using EMRSystem.Emergency.EmergencyCase;
using EMRSystem.Nurses;
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

        public DateTime Time { get; set; } = DateTime.Now;
        public long? NurseId { get; set; } // Who performed the triage
        public virtual Nurse Nurse { get; set; }

        // Vital Signs
        public float? HeartRate { get; set; } // BPM
        public float? BloodPressureSystolic { get; set; } // mmHg
        public float? BloodPressureDiastolic { get; set; } // mmHg
        public float? Temperature { get; set; } // °C or °F
        public float? OxygenSaturation { get; set; } // SpO2 %
        public float? RespiratoryRate { get; set; } // Breaths per minute
        public string Notes { get; set; }
        public EmergencySeverity Severity { get; set; }
    }
}
