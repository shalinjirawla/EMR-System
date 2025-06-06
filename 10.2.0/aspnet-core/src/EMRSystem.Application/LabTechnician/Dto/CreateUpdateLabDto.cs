using Abp.Application.Services.Dto;
using EMRSystem.LabReports;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMRSystem.LabTechnician.Dto
{
    public class CreateUpdateLabDto : EntityDto<long>
    {
        public int TenantId { get; set; }
        public long PatientId { get; set; }
        public string TestName { get; set; }
        public long? RequestedByDoctorId { get; set; }
        public long? CollectedByLabTechId { get; set; }
        public string ResultSummary { get; set; }
        public string FilePath { get; set; } // path to uploaded file
        public DateTime DateConducted { get; set; }
        public LabReportStatus Status { get; set; }
        public string Remarks { get; set; }
    }
}
