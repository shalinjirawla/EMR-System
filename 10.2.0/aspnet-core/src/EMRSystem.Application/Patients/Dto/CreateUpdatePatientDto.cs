using Abp.Application.Services.Dto;
using EMRSystem.Invoices;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMRSystem.Patients.Dto
{
    public class CreateUpdatePatientDto : EntityDto<long>
    {
        public int TenantId { get; set; }

        public string FullName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Gender { get; set; }
        public string Address { get; set; }
        public string BloodGroup { get; set; }
        public string EmergencyContactName { get; set; }
        public string EmergencyContactNumber { get; set; }
        public long? AssignedNurseId { get; set; }
        public BillingMethod BillingMethod { get; set; }
        public PaymentMethod? PaymentMethod { get; set; }
        public long? DepositAmount { get; set; }

        //public bool IsAdmitted { get; set; }
        public DateTime? AdmissionDate { get; set; }
        //public DateTime? DischargeDate { get; set; }
        //public string? InsuranceProvider { get; set; }
        //public string? InsurancePolicyNumber { get; set; }
        public long AbpUserId { get; set; }
        public long? AssignedDoctorId { get; set; }
    }
}
