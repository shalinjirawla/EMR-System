using Abp.Application.Services.Dto;
using EMRSystem.LabReportsType.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMRSystem.LabMasters.Dto.HealthPackage
{
    public class HealthPackageDto : EntityDto<long>
    {
        public int TenantId { get; set; } // Added
        public string PackageName { get; set; }
        public string Description { get; set; }
        public decimal PackagePrice { get; set; }
        public bool IsActive { get; set; } // Added
        public List<long> LabReportsTypeIds { get; set; }

        public List<HealthPackageLabReportsTypeDto> LabReportsTypes { get; set; }
    }
}
