using Abp.Application.Services.Dto;
using EMRSystem.Nurse.Dto;
using EMRSystem.Patients;
using EMRSystem.Patients.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMRSystem.Vitals.Dto
{
    public class VitalDto : EntityDto<long>
    {
        public int TenantId { get; set; }

        public DateTime DateRecorded { get; set; }
        public string BloodPressure { get; set; }  // e.g., "120/80 mmHg"
        public string HeartRate { get; set; }
        public int RespirationRate { get; set; }
        public decimal Temperature { get; set; }
        public int OxygenSaturation { get; set; }
        public string Height { get; set; }
        public string Weight { get; set; }
        public string BMI { get; set; }
        public string Notes { get; set; }
        public PatientDto Patient { get; set; }
        public NurseDto Nurse { get; set; }
    }
}
