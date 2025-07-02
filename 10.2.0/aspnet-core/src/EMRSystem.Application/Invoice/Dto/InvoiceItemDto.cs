using Abp.Application.Services.Dto;
using EMRSystem.Invoices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMRSystem.Invoice.Dto
{
    public class InvoiceItemDto : EntityDto<long>
    {
        public long InvoiceId { get; set; }
        public InvoiceItemType ItemType { get; set; }
        public string ItemTypeDisplay => ItemType.ToString();
        public string Description { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalPrice { get; set; }
    }
}
