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
        public long BillId { get; set; }
        public string Description { get; set; }
        public decimal Amount { get; set; }
    }
}
