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
        //public long? BillingStaffId { get; set; }
        //public long? PharmacyStaffId { get; set; }
        public DateTime? DischargeDate { get; set; }
        public string DischargeSummary { get; set; }
        public DischargeStatus DischargeStatus { get; set; }
    }
}
