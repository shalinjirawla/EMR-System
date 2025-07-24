using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMRSystem.AppointmentType.Dto
{
    public class CreateUpdateAppointmentTypeDto : EntityDto<long>
    {
        public int TenantId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Fee { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
