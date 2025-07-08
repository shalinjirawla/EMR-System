using Abp.Domain.Entities;
using EMRSystem.Pharmacists;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMRSystem.MedicineOrder
{
    public class MedicineOrderItem : Entity<long>
    {
        public long MedicineOrderId { get; set; }
        public virtual MedicineOrder MedicineOrder { get; set; }

        [Required]
        public long MedicineId { get; set; }
        public virtual PharmacistInventory Medicine { get; set; }

        [Required]
        public int Quantity { get; set; }

        public string Dosage { get; set; }
    }
}
