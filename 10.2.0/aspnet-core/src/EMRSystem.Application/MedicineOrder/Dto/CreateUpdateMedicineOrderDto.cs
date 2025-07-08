using Abp.Application.Services.Dto;
using EMRSystem.Nurse.Dto;
using EMRSystem.Patients.Dto;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMRSystem.MedicineOrder.Dto
{
    public class CreateUpdateMedicineOrderDto : EntityDto<long>
    {
        public int TenantId { get; set; }

        [Required]
        public long NurseId { get; set; }

        public long? PatientId { get; set; }

        public string Priority { get; set; }
        public NurseDto Nurse { get; set; }     // Add this
        public PatientDto Patient { get; set; } // Add this

        public List<CreateUpdateMedicineOrderItemDto> Items { get; set; }
    }
}
