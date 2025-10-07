using Abp.Application.Services.Dto;
using EMRSystem.Admissions.Dto;
using EMRSystem.BillingStaff.Dto;
using EMRSystem.Doctor.Dto;
using EMRSystem.PatientDischarge;
using EMRSystem.Patients.Dto;
using EMRSystem.Pharmacist.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMRSystem.Patient_Discharge.Dto
{
    public class PatientDischargeDto : EntityDto<long>
    {
        public int TenantId { get; set; }
        public AdmissionDto Admission { get; set; }
        public PatientDto Patient { get; set; }
        public DoctorDto Doctor { get; set; }
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
        public DoctorDto FollowUpDoctor { get; set; }

    }
}
