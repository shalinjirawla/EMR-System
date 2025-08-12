using Abp.Domain.Entities;
using EMRSystem.LabReportsTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMRSystem.LabMasters
{
    public class HealthPackageLabReportsType : Entity<long>, IMustHaveTenant
    {
        public int TenantId { get; set; }
        public long HealthPackageId { get; set; }
        public virtual HealthPackage HealthPackage { get; set; }
        public long LabReportsTypeId { get; set; }
        public virtual LabReportsType LabReportsType { get; set; }
    }
}
