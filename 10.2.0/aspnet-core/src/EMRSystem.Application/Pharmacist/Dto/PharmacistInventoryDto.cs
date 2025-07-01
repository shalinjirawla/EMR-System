using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMRSystem.Pharmacist.Dto
{
    public class PharmacistInventoryDto : EntityDto<long>
    {
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
        public string StockStatus { get; set; }
        public string ExpiryStatus { get; set; }
    }
}
