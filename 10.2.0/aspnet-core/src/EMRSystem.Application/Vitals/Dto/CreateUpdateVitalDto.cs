using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMRSystem.Vitals.Dto
{
    public class CreateUpdateVitalDto : EntityDto<long>
    {
        public int TenantId { get; set; }

        public DateTime? DateRecorded { get; set; } = DateTime.Now;
        public string BloodPressure { get; set; }  // e.g., "120/80 mmHg"
        public string HeartRate { get; set; }
        public int RespirationRate { get; set; }
        public decimal Temperature { get; set; }
        public int OxygenSaturation { get; set; }
        public string Height { get; set; }
        public string Weight { get; set; }
        public string BMI { get; set; }
        public string Notes { get; set; }
        public long PatientId { get; set; }
        public long NurseId { get; set; }
    }
}
