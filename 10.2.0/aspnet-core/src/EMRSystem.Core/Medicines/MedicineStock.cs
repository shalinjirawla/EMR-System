using Abp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMRSystem.Medicines
{
    public class MedicineStock : Entity<long>, IMustHaveTenant
    {
        public int TenantId { get; set; }

        public long MedicineMasterId { get; set; }
        public virtual MedicineMaster MedicineMaster { get; set; }

        public string BatchNo { get; set; }
        public DateTime? ExpiryDate { get; set; }

        public int Quantity { get; set; }

        public decimal PurchasePrice { get; set; }
        public decimal SellingPrice { get; set; }

        public bool IsExpire { get; set; } = false;
    }
}
