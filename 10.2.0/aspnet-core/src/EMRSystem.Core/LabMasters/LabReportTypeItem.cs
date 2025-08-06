using Abp.Domain.Entities;
using EMRSystem.LabReportsTypes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMRSystem.LabMasters
{
    public class LabReportTypeItem : Entity<long>, IMustHaveTenant
    {
        public int TenantId { get; set; }
        public long LabReportTypeId { get; set; }
        public virtual EMRSystem.LabReportsTypes.LabReportsType LabReportType { get; set; }
        public long LabTestId { get; set; }
        public virtual LabMasters.LabTest LabTest { get; set; }

        public bool IsActive { get; set; } = true;
    }
}
