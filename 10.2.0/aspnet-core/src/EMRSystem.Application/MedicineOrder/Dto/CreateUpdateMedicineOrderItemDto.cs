using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMRSystem.MedicineOrder.Dto
{
    public class CreateUpdateMedicineOrderItemDto : EntityDto<long>
    {
        [Required]
        public long MedicineId { get; set; }

        [Required]
        public int Quantity { get; set; }

        public string Dosage { get; set; }
    }
}
