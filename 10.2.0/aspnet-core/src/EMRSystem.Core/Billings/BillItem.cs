using Abp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMRSystem.Billings
{
    public class BillItem : Entity<long>
    {
        public string Description { get; set; }
        public int Quntity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Amount { get; set; }
        public long BillId { get; set; }
        public Bill Bill { get; set; }
    }
}
