using Abp.Domain.Entities;
using EMRSystem.IpdChargeEntry;
using EMRSystem.Patients;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMRSystem.EmergencyChargeEntries
{
    public class EmergencyChargeEntry : Entity<long>, IMustHaveTenant
    {
        public int TenantId { get; set; }
        public long? PatientId { get; set; }
        public ChargeType ChargeType { get; set; }
        public string Description { get; set; }
        public decimal Amount { get; set; }
        public DateTime EntryDate { get; set; } = DateTime.Now;
        public bool IsProcessed { get; set; } = false;
        public long? ReferenceId { get; set; }
    }
}
