using Abp.Domain.Entities;
using System;

namespace EMRSystem.Invoices
{
    public class InvoiceItem : Entity<long>
    {
        public long InvoiceId { get; set; }
        public virtual Invoice Invoice { get; set; }
        public InvoiceItemType ItemType { get; set; }
        public string Description { get; set; }
        public int Quantity { get; set; } = 1;
        public decimal UnitPrice { get; set; }


       
    }

    public enum InvoiceItemType
    {
        Consultation,
        Medicine,
        LabTest
    }
}