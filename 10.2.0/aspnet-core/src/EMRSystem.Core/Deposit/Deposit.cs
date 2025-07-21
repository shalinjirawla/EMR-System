using Abp.Domain.Entities;
using EMRSystem.Invoices;
using EMRSystem.Patients;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMRSystem.Deposit
{
    public class Deposit : Entity<long>, IMustHaveTenant
    {
        public int TenantId { get; set; }
        public long PatientId { get; set; }
        public decimal Amount { get; set; }
        public PaymentMethod PaymentMethod { get; set; }
        public BillingMethod BillingMethod { get; set; }
        public DateTime DepositDateTime { get; set; } 

        // Navigation
        //public virtual EMRSystem.Admission.Admission Admission { get; set; }
        public virtual EMRSystem.Patients.Patient Patient { get; set; }

    }
}
