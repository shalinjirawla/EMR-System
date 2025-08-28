using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMRSystem.EmergencyProcedure.Dto
{
    public class EmergencyProcedureDto : EntityDto<long>
    {
        public int TenantId { get; set; }
        public string Name { get; set; }
        public ProcedureCategory Category { get; set; }
        public decimal DefaultCharge { get; set; }
        public bool IsActive { get; set; }
    }
}
