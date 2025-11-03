using Abp.Domain.Entities;
using EMRSystem.Invoices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMRSystem.Deposit
{
    public class DepositTransaction : Entity<long>, IMustHaveTenant
    {
        public int TenantId { get; set; }
        public long PatientDepositId { get; set; }
        public decimal Amount { get; set; }
        public TransactionType TransactionType { get; set; }
        public PaymentMethod? PaymentMethod { get; set; }
        public DateTime TransactionDate { get; set; }
        public string Description { get; set; }
        public string? ReceiptNo { get; set; }
        public bool IsPaid { get; set; }
        public string? PaymentIntentId { get; set; }

        // ✅ Refund tracking fields (as per your request)
        public decimal RemainingAmount { get; set; }            // Kitna balance bacha tha
        public decimal RefundedAmount { get; set; }            // Kitna refund diya
        public bool IsRefund { get; set; }                     // Ye row refund hai?
        public string? RefundTransactionId { get; set; }       // Stripe refund id OR internal refund id
        public DateTime? RefundDate { get; set; }              // Refund kab hua
        public string? RefundReceiptNo { get; set; }           // Refund receipt number

        // Navigation
        public virtual PatientDeposit PatientDeposit { get; set; }
    }
    public enum TransactionType
    {
        Credit,
        Debit
    }
}
