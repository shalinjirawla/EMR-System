using Abp.Application.Services.Dto;
using EMRSystem.LabReports;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMRSystem.LabTechnician.Dto
{
    public class CreateUpdateLabReportDto : EntityDto<long>
    {
        public int TenantId { get; set; }

        public long PatientId { get; set; }
        public string TestName { get; set; }
        public string ResultSummary { get; set; }
        public string Remarks { get; set; }
        public LabReportStatus Status { get; set; }
        public long LabTechnicianId { get; set; }
    }
}
