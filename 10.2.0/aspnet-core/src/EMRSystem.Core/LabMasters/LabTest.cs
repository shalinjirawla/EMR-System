using Abp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMRSystem.LabMasters
{
    public class LabTest : Entity<long>, IMustHaveTenant
    {
        public int TenantId { get; set; }
        public string Name { get; set; }
        public long? MeasureUnitId { get; set; }
        public virtual EMRSystem.LabMasters.MeasureUnit MeasureUnit { get; set; }
        public virtual ICollection<LabReportTypeItem> LabReportTypeItems { get; set; }
        public virtual ICollection<TestResultLimit> TestResultLimits { get; set; }


        public bool IsActive { get; set; } = true;
    }
}
