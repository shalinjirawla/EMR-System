﻿using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMRSystem.Billings
{
    public class Bill : FullAuditedEntity<long>, IMustHaveTenant
    {
        public int TenantId { get; set; }
        public long PatientId { get; set; }
        public long? GeneratedByUserId { get; set; } // Billing staff
        public DateTime BillDate { get; set; }
        public decimal TotalAmount { get; set; }
        public PaymentStatus PaymentStatus { get; set; }
        public string PaymentMethod { get; set; } // e.g. Cash, Insurance, Card
        public ICollection<BillItem> Items { get; set; }
    }
    public enum PaymentStatus
    {
        Pending,
        Paid,
        PartiallyPaid
    }
}
