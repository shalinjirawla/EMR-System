using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMRSystem.LabMasters.Dto.HealthPackage
{
    public class CreateUpdateHealthPackageDto:EntityDto<long>
    {
        public int TenantId { get; set; }
        public string PackageName { get; set; }
        public string Description { get; set; }
        public decimal PackagePrice { get; set; }
        public bool IsActive { get; set; } // Added

        public List<CreateUpdateHealthPackageLabReportsTypeDto> LabReportsTypes { get; set; }
    }
}
