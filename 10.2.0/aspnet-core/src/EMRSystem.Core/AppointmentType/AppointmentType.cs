using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMRSystem.AppointmentType
{
    public class AppointmentType : Entity<long>, IMustHaveTenant
    {
        public int TenantId { get; set; }
        public string Name { get; set; } // e.g., "Consultation", "Follow-up", "Lab Test"
        public string Description { get; set; }
        public decimal Fee { get; set; } // Monetary value for this appointment type
        public bool IsActive { get; set; } = true;
        public DateTime CreationTime { get; set; } = DateTime.Now;
    }
}
