using EMRSystem.Prescriptions.Dto;
using EMRSystem.Vitals.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMRSystem.Patient_Discharge.Dto
{
    public class DischargeSummaryDto
    {
        public PatientDetailsFordischargeSummaryDto PatientDetails { get; set; } = new();
        public List<VitalDto> Vitals { get; set; } = null;
        //public List<ViewPrescriptionSummary> Prescriptions { get; set; } = null;
    }
    
}
