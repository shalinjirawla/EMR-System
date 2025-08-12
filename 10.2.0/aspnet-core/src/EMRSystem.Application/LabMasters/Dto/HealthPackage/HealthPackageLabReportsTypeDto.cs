using Abp.Application.Services.Dto;
using EMRSystem.LabReportsType.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMRSystem.LabMasters.Dto.HealthPackage
{
    public class HealthPackageLabReportsTypeDto : EntityDto<long>
    {
        public int TenantId { get; set; } // Added TenantId

        public long HealthPackageId { get; set; }
        public long LabReportsTypeId { get; set; }

        public LabReportsTypeDto LabReportsType { get; set; }
    }
}
