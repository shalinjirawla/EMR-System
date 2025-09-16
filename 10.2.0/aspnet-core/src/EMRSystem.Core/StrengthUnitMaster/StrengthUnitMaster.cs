using Abp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMRSystem.StrengthUnitMaster
{
    public class StrengthUnitMaster : Entity<long>, IMustHaveTenant
    {
        public int TenantId { get; set; }

        [Required]
        [StringLength(50)]
        public string Name { get; set; }  // mg, ml, g, IU
        public bool IsActive { get; set; } = true;

    }
}
