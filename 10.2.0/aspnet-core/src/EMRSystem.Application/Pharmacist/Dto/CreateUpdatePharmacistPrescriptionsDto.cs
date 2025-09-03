using Abp.Application.Services.Dto;
using EMRSystem.MedicineOrder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMRSystem.Pharmacist.Dto
{
    public class CreateUpdatePharmacistPrescriptionsDto : EntityDto<long>
    {
        public int TenantId { get; set; }
        public long? PrescriptionId { get; set; }
        public DateTime? IssueDate { get; set; }
        public OrderStatus Order_Status { get; set; }
    }
}
