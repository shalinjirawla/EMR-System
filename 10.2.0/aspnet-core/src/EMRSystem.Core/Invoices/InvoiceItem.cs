using Abp.Domain.Entities;
using EMRSystem.IpdChargeEntry;
using System;

namespace EMRSystem.Invoices
{
    public class InvoiceItem : Entity<long>
    {
        public long InvoiceId { get; set; }
        public virtual Invoice Invoice { get; set; }
        //public ChargeType ItemType { get; set; }
        public string Description { get; set; }
        public int Quantity { get; set; } = 1;
        public decimal UnitPrice { get; set; }



    }
}