using Abp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMRSystem.BillingStaff.Dto
{
    public class BillItemDto : Entity<long>
    {
        public string Description { get; set; }
        public int Quntity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Amount { get; set; }
        public long BillId { get; set; }
        public BillingDto Bills { get; set; }
    }
}
