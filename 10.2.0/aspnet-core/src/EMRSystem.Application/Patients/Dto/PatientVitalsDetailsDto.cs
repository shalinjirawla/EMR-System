using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMRSystem.Patients.Dto
{
    public class PatientVitalsDetailsDto
    {
        public string BloodPressure { get; set; }
        public string HeartRate { get; set; }
        public int RespirationRate { get; set; }
        public decimal Temperature { get; set; }
        public int OxygenSaturation { get; set; }
        public string Height { get; set; }
        public string Weight { get; set; }
        public string BMI { get; set; }
        public DateTime? DateRecorded { get; set; }
    }
}
