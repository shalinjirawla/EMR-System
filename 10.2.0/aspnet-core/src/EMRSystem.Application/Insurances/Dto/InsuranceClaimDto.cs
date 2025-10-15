using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMRSystem.Insurances.Dto
{
    public class InsuranceClaimDto : EntityDto<long>
    {
        public int TenantId { get; set; }
        public long InvoiceId { get; set; }
        public long? PatientInsuranceId { get; set; }
        public string InsuranceName{ get; set; }
        public string InvoiceNo { get; set; }
        public string PatientName{ get; set; }


        public decimal TotalAmount { get; set; }
        public decimal AmountPayByInsurance { get; set; }
        public decimal AmountPayByPatient { get; set; }

        public string Remarks { get; set; }
        public ClaimStatus Status { get; set; }

        public DateTime? SubmittedOn { get; set; }
        public DateTime? RespondedOn { get; set; }
        public DateTime? PaidOn { get; set; }
    }
}
