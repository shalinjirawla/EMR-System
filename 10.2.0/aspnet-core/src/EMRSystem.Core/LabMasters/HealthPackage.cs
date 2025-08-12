using Abp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMRSystem.LabMasters
{
    public class HealthPackage : Entity<long>, IMustHaveTenant
    {
        public int TenantId { get; set; }
        public string PackageName { get; set; }
        public string Description { get; set; }
        public decimal PackagePrice { get; set; }
        public bool IsActive { get; set; }
        public virtual ICollection<HealthPackageLabReportsType> PackageReportTypes { get; set; }
    }
}
