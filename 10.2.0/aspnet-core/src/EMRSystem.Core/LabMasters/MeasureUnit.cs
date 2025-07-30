using Abp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMRSystem.LabMasters
{
    public class MeasureUnit : Entity<long>, IMustHaveTenant
    {
        public int TenantId { get; set; }
        public string Name { get; set; }
        public bool IsActive { get; set; } = true;
        public virtual ICollection<LabTest> LabTests { get; set; }
    }
}
