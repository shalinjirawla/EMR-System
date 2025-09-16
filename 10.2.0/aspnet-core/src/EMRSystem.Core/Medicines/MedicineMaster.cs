using Abp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMRSystem.Medicines
{
    public class MedicineMaster : Entity<long>
    {
        public int TenantId { get; set; }

        public string Name { get; set; }

        public long MedicineFormId { get; set; }
        public virtual EMRSystem.MedicineFormMaster.MedicineFormMaster Form { get; set; }
        public decimal? Strength { get; set; }
        public long? StrengthUnitId { get; set; }
        public virtual EMRSystem.StrengthUnitMaster.StrengthUnitMaster StrengthUnit { get; set; }

        public int MinimumStock { get; set; }

        public string? Description { get; set; }

        public bool IsAvailable { get; set; } = true;

        public virtual ICollection<MedicineStock> Stocks { get; set; }
    }
}
