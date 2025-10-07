using Abp.Application.Services.Dto;
using EMRSystem.PatientDischarge;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMRSystem.Patient_Discharge.Dto
{
    public class CreateUpdatePatientDischargeDto : EntityDto<long>
    {
        public int TenantId { get; set; }
        public long AdmissionId { get; set; }
        public long PatientId { get; set; }
        public long? DoctorId { get; set; }
        public DateTime? DischargeDate { get; set; }
        public string DischargeSummary { get; set; }
        public DischargeStatus DischargeStatus { get; set; }
        public string? ProvisionalDiagnosis { get; set; }
        public string? FinalDiagnosis { get; set; }
        public string? InvestigationSummary { get; set; }
        public string? ConditionAtDischarge { get; set; }
        public string? DietAdvice { get; set; }
        public string? Activity { get; set; }
        public DateTime? FollowUpDate { get; set; }
        public long? FollowUpDoctorId { get; set; }
        public string DoctorName { get; set; }
        public string FollowUpDoctorName { get; set; }
        public string DoctorQualification { get; set; }
    }
}
