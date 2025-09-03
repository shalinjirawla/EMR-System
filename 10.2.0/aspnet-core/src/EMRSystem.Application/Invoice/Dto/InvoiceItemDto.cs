using Abp.Application.Services.Dto;
using EMRSystem.Invoices;
using EMRSystem.IpdChargeEntry;
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
        //public ChargeType ItemType { get; set; }
        //public string ItemTypeDisplay => ItemType.ToString();
        public string Description { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public DateTime EntryDate { get; set; } = DateTime.Now;

        //public decimal TotalPrice { get; set; }
    }
}
