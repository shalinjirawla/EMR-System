using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMRSystem.MedicineForms.Dto
{
    public class CreateUpdateMedicineFormMasterDto : EntityDto<long>
    {
        public int TenantId { get; set; }
        [Required]
        [StringLength(100)]
        public string Name { get; set; }
        public bool IsActive { get; set; } = true;

    }
}
