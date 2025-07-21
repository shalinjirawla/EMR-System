using Abp.Application.Services.Dto;
using EMRSystem.Invoices;
using EMRSystem.Patients;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMRSystem.Deposit.Dto
{
    public class DepositDto : EntityDto<long>
    {
        public int TenantId { get; set; }
        public long PatientId { get; set; }
        public string PatientName { get; set; }
        public decimal Amount { get; set; }

        public PaymentMethod PaymentMethod { get; set; }
        public BillingMethod BillingMethod { get; set; }

        public DateTime DepositDateTime { get; set; }
    }
}
