using Abp.Application.Services.Dto;
using EMRSystem.Emergency.EmergencyCase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMRSystem.Emergency.Triage
{
    public class TriageDto : EntityDto<long>
    {
        public int TenantId { get; set; }
        public long EmergencyCaseId { get; set; }
        public string EmergencyNumber { get; set; }
        public DateTime Time { get; set; }
        public long? NurseId { get; set; }
        public string NurseName { get; set; }
        public float? HeartRate { get; set; }
        public float? BloodPressureSystolic { get; set; }
        public float? BloodPressureDiastolic { get; set; }
        public string BloodPressureDisplay =>
            BloodPressureSystolic.HasValue && BloodPressureDiastolic.HasValue
            ? $"{BloodPressureSystolic}/{BloodPressureDiastolic}"
            : null;
        public float? Temperature { get; set; }
        public float? OxygenSaturation { get; set; }

        public float? RespiratoryRate { get; set; } // Breaths per minute
        public string Notes { get; set; }

        public EmergencySeverity Severity { get; set; }
    }
}
