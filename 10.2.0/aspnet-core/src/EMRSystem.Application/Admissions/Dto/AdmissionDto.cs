using Abp.Application.Services.Dto;
using EMRSystem.Admission;
using EMRSystem.Patients;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMRSystem.Admissions.Dto
{
    public class AdmissionDto : EntityDto<long>
    {
        public int TenantId { get; set; }
        public long PatientId { get; set; }
        public string PatientName { get; set; }
        public DateTime AdmissionDateTime { get; set; }
        public long DoctorId { get; set; }
        public string DoctorName { get; set; }
        public long? NurseId { get; set; }
        public string NurseName { get; set; }
        public long RoomId { get; set; }
        public long? BedId { get; set; }
        public string BedNumber { get; set; }
        public string RoomNumber { get; set; }
        public string RoomTypeName { get; set; }
        public decimal? RoomTypePricePerDay { get; set; }
        public string ReasonForAdmit { get; set; }

        public bool IsDischarged { get; set; }
        public AdmissionType AdmissionType { get; set; }
        public BillingMethod BillingMode { get; set; }
        public long? PatientInsuranceId { get; set; }
        public long InsuranceId { get; set; }
        public string InsuranceName { get; set; }
        public string PolicyNumber { get; set; }
        public decimal? CoPayPercentage { get; set; }
        public decimal? CoverageLimit { get; set; }
    }
}
