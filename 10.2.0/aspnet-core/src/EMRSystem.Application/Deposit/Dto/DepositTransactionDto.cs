using Abp.Application.Services.Dto;
using EMRSystem.Invoices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMRSystem.Deposit.Dto
{
    public class DepositTransactionDto : EntityDto<long>
    {
        public int TenantId { get; set; }
        public long PatientDepositId { get; set; }
        public decimal Amount { get; set; }
        public TransactionType TransactionType { get; set; }
        public PaymentMethod PaymentMethod { get; set; }
        public DateTime TransactionDate { get; set; }
        public string Description { get; set; }
        public string ReceiptNo { get; set; }
        public bool IsPaid { get; set; }
        public string? PaymentIntentId { get; set; }

    }
}
