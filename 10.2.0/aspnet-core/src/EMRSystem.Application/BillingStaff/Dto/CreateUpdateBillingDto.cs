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
    public class CreateUpdateBillingDto : EntityDto<long>
    {
        public long PatientId { get; set; }
        public DateTime BillDate { get; set; }
        public DateTime AdmissionDate { get; set; }
        public DateTime DateOfSurgery { get; set; }
        public decimal TotalAmount { get; set; }
        public PaymentStatus PaymentStatus { get; set; }
        public string PaymentMethod { get; set; } // e.g. Cash, Insurance, Card
        public ICollection<BillItemDto> Items { get; set; }
        public long AbpUserId { get; set; }
    }
}
