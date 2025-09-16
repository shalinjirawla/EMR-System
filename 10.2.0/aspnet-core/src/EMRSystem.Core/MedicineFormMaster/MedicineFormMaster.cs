using Abp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMRSystem.MedicineFormMaster
{
    public class MedicineFormMaster : Entity<long>, IMustHaveTenant
    {
        public int TenantId { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }  // Tablet, Syrup, Injection etc.
        public bool IsActive{ get; set; } = true;

    }
}
