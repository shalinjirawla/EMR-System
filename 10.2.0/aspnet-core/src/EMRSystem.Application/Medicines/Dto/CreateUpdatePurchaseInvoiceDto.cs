using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMRSystem.Medicines.Dto
{
    public class CreateUpdatePurchaseInvoiceDto : EntityDto<long>
    {
        public int TenantId { get; set; }

        public string InvoiceNo { get; set; }
        public DateTime InvoiceDate { get; set; }
        public string SupplierName { get; set; }
        public decimal TotalAmount { get; set; }

        public List<CreateUpdatePurchaseInvoiceItemDto> Items { get; set; }
    }
}
