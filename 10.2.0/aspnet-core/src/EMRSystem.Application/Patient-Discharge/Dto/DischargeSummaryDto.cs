using Abp.Application.Services.Dto;
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
        public PatientDischargeDto PatientDischarge { get; set; }
        public PatientDetailsFordischargeSummaryDto PatientDetails { get; set; } = new();
        public List<VitalDto> Vitals { get; set; } = null;
        public List<PrescriptionDto> Prescriptions { get; set; }
        public List<EMRSystem.PrescriptionLabTest.Dto.PrescriptionLabTestDto> PrescriptionLabTests { get; set; }
        public List<EMRSystem.EmergencyProcedure.Dto.SelectedEmergencyProceduresDto> SelectedEmergencyProcedures { get; set; }
        public List<EMRSystem.Invoice.Dto.InvoiceDto> Invoices { get; set; }
    }
    
}
