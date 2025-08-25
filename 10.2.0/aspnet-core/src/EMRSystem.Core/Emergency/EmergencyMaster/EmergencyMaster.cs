using Abp.Domain.Entities;
using EMRSystem.Doctors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMRSystem.Emergency.EmergencyMaster
{
    public class EmergencyMaster : Entity<long>, IMustHaveTenant
    {
        public int TenantId { get; set; }
        public string Title { get; set; }
        public decimal Fee { get; set; }
    }
}
