using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMRSystem.EmergencyMaster.Dto
{
    public class CreateUpdateEmergencyMasterDto : EntityDto<long>
    {
        public int TenantId { get; set; }
        public string Title { get; set; }
        public decimal Fee { get; set; }
    }
}
