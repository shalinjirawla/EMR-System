using Abp.Domain.Entities;
using EMRSystem.Patients;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace EMRSystem.Deposit
{
    public class PatientDeposit : Entity<long>, IMustHaveTenant
    {
        public int TenantId { get; set; }
        public long? PatientId { get; set; }

        public decimal TotalCreditAmount { get; set; }
        public decimal TotalDebitAmount { get; set; }
        public decimal TotalBalance { get; set; }
        public virtual Patient Patient { get; set; }

        public virtual ICollection<DepositTransaction> Transactions { get; set; }
    }
}
