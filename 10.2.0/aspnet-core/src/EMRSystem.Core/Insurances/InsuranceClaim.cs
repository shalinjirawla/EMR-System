using Abp.Domain.Entities;
using EMRSystem.Invoices;
using System;

namespace EMRSystem.Insurances
{
    public class InsuranceClaim : Entity<long>, IMustHaveTenant
    {
        public int TenantId { get; set; }

        // Link to Invoice
        public long InvoiceId { get; set; }
        public virtual Invoice Invoice { get; set; }

        // Link to PatientInsurance (contains both patient & insurance)
        public long PatientInsuranceId { get; set; }
        public virtual PatientInsurance PatientInsurance { get; set; }

        public decimal TotalAmount { get; set; }            // Invoice total
        public decimal AmountPayByInsurance { get; set; }   // Approved by insurer (updated later)
        public decimal AmountPayByPatient { get; set; }     // Co-pay / non-covered (updated later)

        public ClaimStatus Status { get; set; } = ClaimStatus.Pending;
        public string Remarks { get; set; }                 // Notes from billing staff / insurer

        // Dates
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public DateTime? SubmittedOn { get; set; }
        public DateTime? RespondedOn { get; set; }
        public DateTime? PaidOn { get; set; }
    }

    public enum ClaimStatus
    {
        Pending = 0,
        Submitted = 1,
        PartialApproved = 2,
        Approved = 3,
        Rejected = 4,
        Paid = 5
    }
}
