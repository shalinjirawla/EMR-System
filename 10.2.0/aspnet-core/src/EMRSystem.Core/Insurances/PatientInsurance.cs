using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using EMRSystem.Patients;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMRSystem.Insurances
{
    public class PatientInsurance : Entity<long>, IMustHaveTenant
    {
        public int TenantId { get; set; }
        public long PatientId { get; set; }
        public virtual Patient Patient { get; set; }
        public long InsuranceId { get; set; }
        public virtual InsuranceMaster InsuranceMaster { get; set; }
        public ICollection<InsuranceClaim> InsuranceClaims { get; set; }


        public string PolicyNumber { get; set; }
        public decimal CoverageLimit { get; set; } // max sum insured
        public decimal? CoPayPercentage { get; set; } // optional, null = 100% insurance cover

        public bool IsActive { get; set; } = true;

    }

}
