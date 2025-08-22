using Abp.Application.Services.Dto;
using EMRSystem.Emergency.EmergencyCase;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMRSystem.Emergency.Triage
{
    public class CreateUpdateTriageDto : EntityDto<long>
    {
        public int TenantId { get; set; }
        public DateTime Time { get; set; } = DateTime.Now;
        [Required]
        public long EmergencyCaseId { get; set; }
        public float? HeartRate { get; set; } // BPM
        public float? BloodPressureSystolic { get; set; } // mmHg
        public float? BloodPressureDiastolic { get; set; } // mmHg
        public float? Temperature { get; set; } // °C or °F
        public float? OxygenSaturation { get; set; } // SpO2 %
        public float? RespiratoryRate { get; set; } // Breaths per minute
        public string Notes { get; set; }
        [Required]
        public EmergencySeverity Severity { get; set; }
    }
}
