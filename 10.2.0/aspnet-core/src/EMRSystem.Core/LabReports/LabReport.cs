using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMRSystem.LabReports
{
    public class LabReport : FullAuditedEntity<long>, IMustHaveTenant
    {
        public int TenantId { get; set; }
        public long PatientId { get; set; }
        public string TestName { get; set; }
        public string ResultSummary { get; set; }
        //public string FilePath { get; set; } // path to uploaded file
        public string Remarks { get; set; }
        public LabReportStatus Status { get; set; }
        public long LabTechnicianId { get; set; }
        public virtual LabTechnician LabTechnicians { get; set; }
    }

    public enum LabReportStatus
    {
        Pending,
        Completed
    }
}
