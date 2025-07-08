using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMRSystem.MedicineOrder.Dto
{
    public class MedicineOrderItemDto : EntityDto<long>
    {
        public long MedicineOrderId { get; set; }

        public long MedicineId { get; set; }

        public string MedicineName { get; set; } // Optional for read purposes

        public int Quantity { get; set; }

        public string Dosage { get; set; }
    }
}
