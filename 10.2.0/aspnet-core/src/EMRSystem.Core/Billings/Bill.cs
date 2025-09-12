using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using EMRSystem.Authorization.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMRSystem.Billings
{
    public class Bill : Entity<long>, IMustHaveTenant
    {
        public int TenantId { get; set; }
        public long PatientId { get; set; }
        public DateTime BillDate { get; set; }
        public DateTime AdmissionDate { get; set; }
        public DateTime DateOfSurgery { get; set; }
        public decimal TotalAmount { get; set; }
        public PaymentStatus PaymentStatus { get; set; }
        public string PaymentMethod { get; set; } // e.g. Cash, Insurance, Card
        public ICollection<BillItem> Items { get; set; }
        public long AbpUserId { get; set; }
        public virtual User AbpUser { get; set; }
        public virtual ICollection<EMRSystem.PatientDischarge.PatientDischarge> _PatientDischarge { get; set; }
    }
    public enum PaymentStatus
    {
        Pending,
        Paid,
        PartiallyPaid
    }
}
