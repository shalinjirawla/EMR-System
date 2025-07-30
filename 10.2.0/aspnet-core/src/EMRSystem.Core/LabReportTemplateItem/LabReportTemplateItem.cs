using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using EMRSystem.LabReportsTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMRSystem.LabReportTemplateItem
{
    public class LabReportTemplateItem : Entity<long>, IMustHaveTenant
    {
        public int TenantId { get; set; }

        public string Test { get; set; }

        public decimal? Result { get; set; }

        public string? Unit { get; set; }

        public decimal? MinValue { get; set; }

        public decimal? MaxValue { get; set; }

        public long LabReportsTypeId { get; set; }

        public LabReportsType LabReportsType { get; set; }
    }
}
