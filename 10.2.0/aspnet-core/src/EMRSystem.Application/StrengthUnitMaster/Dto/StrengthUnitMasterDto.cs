using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMRSystem.StrengthUnitMaster.Dto
{
    public class StrengthUnitMasterDto : EntityDto<long>
    {
        public int TenantId { get; set; }

        [Required]
        [StringLength(50)]
        public string Name { get; set; }
        public bool IsActive { get; set; } = true;

    }
}
