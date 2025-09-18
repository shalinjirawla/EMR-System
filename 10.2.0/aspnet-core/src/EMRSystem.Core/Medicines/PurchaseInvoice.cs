using Abp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMRSystem.Medicines
{
    public class PurchaseInvoice : Entity<long>, IMustHaveTenant
    {
        public int TenantId { get; set; }

        public string InvoiceNo { get; set; }
        public DateTime InvoiceDate { get; set; }

        public string SupplierName { get; set; }   // Future me SupplierMasterId use kar sakte ho

        public decimal TotalAmount { get; set; }

        public virtual ICollection<PurchaseInvoiceItem> Items { get; set; }
    }
}
