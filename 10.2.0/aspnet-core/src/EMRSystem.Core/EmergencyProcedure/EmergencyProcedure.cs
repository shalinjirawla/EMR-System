using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMRSystem.EmergencyProcedure
{
    public class EmergencyProcedure : Entity<long>, IMustHaveTenant
    {
        public int TenantId { get; set; }

        public string Name { get; set; } // e.g. Nebulization, Intubation

        public ProcedureCategory Category { get; set; } // Minor, Major, LifeSaving

        public decimal DefaultCharge { get; set; }

        public bool IsActive { get; set; }
    }
    public enum ProcedureCategory
    {
        Minor,
        Major,
        LifeSaving
    }
}
