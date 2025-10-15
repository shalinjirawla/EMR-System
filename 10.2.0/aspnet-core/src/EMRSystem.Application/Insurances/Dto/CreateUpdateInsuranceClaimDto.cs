using Abp.Application.Services.Dto;
using EMRSystem.Invoice.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMRSystem.Insurances.Dto
{
    public class CreateUpdateInsuranceClaimDto : EntityDto<long>
    {
        public int TenantId { get; set; }
        public long InvoiceId { get; set; }
        public long? PatientInsuranceId { get; set; }

        public decimal TotalAmount { get; set; }
        public decimal AmountPayByInsurance { get; set; }
        public decimal AmountPayByPatient { get; set; }

        public ClaimStatus Status { get; set; } = ClaimStatus.Pending;

        public List<InvoiceItemDto> Items { get; set; } = new List<InvoiceItemDto>();
    }
}
