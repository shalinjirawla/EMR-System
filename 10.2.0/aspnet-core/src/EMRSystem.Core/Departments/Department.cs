using Abp.Domain.Entities;
using EMRSystem.Visits;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMRSystem.Departments
{
    public class Department : Entity<long>, IMustHaveTenant
    {
        public int TenantId { get; set; }
        public string Name { get; set; }
        public ICollection<Visit> Visits { get; set; }
    }
}
