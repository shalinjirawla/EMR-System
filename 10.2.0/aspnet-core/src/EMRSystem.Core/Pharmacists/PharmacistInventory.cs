using Abp.Domain.Entities;
using Abp.Domain.Uow;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMRSystem.Pharmacists
{
    public class PharmacistInventory : Entity<long>, IMustHaveTenant
    {
        // Existing properties
        public int TenantId { get; set; }
        public string MedicineName { get; set; }
        public decimal CostPrice { get; set; }
        public decimal SellingPrice { get; set; }
        public DateTime ExpiryDate { get; set; }
        public DateTime PurchaseDate { get; set; }
        public string Unit { get; set; }
        public int Stock { get; set; }
        public int MinStock { get; set; }
        public string Description { get; set; }
        public bool IsAvailable { get; set; }

        // Auto-calculated stock status
        public string StockStatus =>
            Stock <= 0 ? "Out of Stock" :
            Stock <= MinStock ? "Low Stock" :
            "In Stock";

        // Auto-calculated expiry status
        public string ExpiryStatus =>
            ExpiryDate < DateTime.Today ? "Expired" :
            (ExpiryDate - DateTime.Today).TotalDays <= 30 ? "Expires Soon" :
            "Valid";
    }
}
