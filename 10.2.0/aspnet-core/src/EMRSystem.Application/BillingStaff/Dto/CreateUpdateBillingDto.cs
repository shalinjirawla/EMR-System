using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using EMRSystem.Billings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMRSystem.BillingStaff.Dto
{
    [AutoMapTo(typeof(EMRSystem.Billings.Bill))]
    public class CreateUpdateBillingDto : EntityDto<long>
    {
        public long PatientId { get; set; }
        public long? GeneratedByUserId { get; set; } // Billing staff
        public DateTime BillDate { get; set; }
        public decimal TotalAmount { get; set; }
        public PaymentStatus PaymentStatus { get; set; }
        public string PaymentMethod { get; set; } // e.g. Cash, Insurance, Card
        public ICollection<BillItemDto> Items { get; set; }
    }
}
