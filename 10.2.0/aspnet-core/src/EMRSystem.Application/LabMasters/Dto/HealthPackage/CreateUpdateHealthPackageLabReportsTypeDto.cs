using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMRSystem.LabMasters.Dto.HealthPackage
{
    public class CreateUpdateHealthPackageLabReportsTypeDto
    {
        public int TenantId { get; set; } // Added TenantId

        public long LabReportsTypeId { get; set; }
    }
}
