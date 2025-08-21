using Abp.Application.Services.Dto;
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
        //public string TriageLevel { get; set; }
        public string Notes { get; set; }
        public decimal? Temperature { get; set; }
        public int? Pulse { get; set; }
        public int? RespiratoryRate { get; set; }
        public int? BloodPressureSystolic { get; set; }
        public int? BloodPressureDiastolic { get; set; }
        public DateTime AssessmentTime { get; set; }
    }
}
