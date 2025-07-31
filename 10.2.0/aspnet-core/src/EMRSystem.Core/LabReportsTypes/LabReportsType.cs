using Abp.Domain.Entities;
using EMRSystem.LabReports;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMRSystem.LabReportsTypes
{
    public class LabReportsType : Entity<long>, IMustHaveTenant
    {
        public int TenantId { get; set; }
        public string ReportType { get; set; }
        public int ReportPrice { get; set; }
        public virtual ICollection<PrescriptionLabTest> PrescriptionLabTests { get; set; }
        public ICollection<EMRSystem.LabReportTemplateItem.LabReportTemplateItem> TemplateItems { get; set; }

    }
}
