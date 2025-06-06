using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMRSystem.Inventory
{
    public class InventoryItem : FullAuditedEntity<long>, IMustHaveTenant
    {
        public int TenantId { get; set; }
        public string Name { get; set; }
        public string Type { get; set; } // Medicine, Equipment, etc.
        public int Quantity { get; set; }
        public string BatchNumber { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public string Manufacturer { get; set; }
        public decimal UnitPrice { get; set; }
        public long? AddedByUserId { get; set; }
    }
}
