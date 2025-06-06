using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMRSystem.Vitals
{
    public class Vital : FullAuditedEntity<long>, IMustHaveTenant
    {
        public int TenantId { get; set; }
        public long PatientId { get; set; }
        public long NurseId { get; set; }
        public DateTime DateRecorded { get; set; }
        public decimal Temperature { get; set; }
        public int Pulse { get; set; }
        public string BloodPressure { get; set; } // Format: "120/80"
        public int RespirationRate { get; set; }
        public int OxygenSaturation { get; set; }
    }
}
