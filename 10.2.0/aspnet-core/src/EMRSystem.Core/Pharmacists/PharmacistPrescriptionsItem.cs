using Abp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMRSystem.Pharmacists
{
    public class PharmacistPrescriptionsItem : Entity<long>, IMustHaveTenant
    {
        public int TenantId { get; set; }
        public long PharmacistPrescriptionId { get; set; }
        public EMRSystem.Pharmacists.PharmacistPrescriptions PharmacistPrescription { get; set; }
        public decimal GrandTotal { get; set; }
        public DateTime? CreatedAt { get; set; }
    }
}
