using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMRSystem.Medicines.Dto
{
    public class MedicineStockDto : EntityDto<long>
    {
        public int TenantId { get; set; }
        public long MedicineMasterId { get; set; }
        public string MedicineName { get; set; }
        public string BatchNo { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public int Quantity { get; set; }
        public decimal PurchasePrice { get; set; }
        public decimal SellingPrice { get; set; }
        public bool IsExpire { get; set; }
        public int TotalStock { get; set; }       // All batches combined
        public string StockStatus { get; set; }   // Out of Stock / Low Stock / In Stock
        public string ExpiryStatus { get; set; }
        public int? DaysToExpire { get; set; }

    }
}
