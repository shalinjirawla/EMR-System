using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMRSystem.Medicines.Dto
{
    public class PurchaseInvoiceItemDto : EntityDto<long>
    {
        public int TenantId { get; set; }

        public long PurchaseInvoiceId { get; set; }
        public long MedicineMasterId { get; set; }

        public string BatchNo { get; set; }
        public DateTime? ExpiryDate { get; set; }

        public int Quantity { get; set; }
        public decimal PurchasePrice { get; set; }
        public decimal SellingPrice { get; set; }
    }
}
