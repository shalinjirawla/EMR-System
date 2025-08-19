using Abp.Application.Services.Dto;
using EMRSystem.LabReport.Dto;
using EMRSystem.LabReports;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMRSystem.PrescriptionLabTest.Dto
{
    public class PrescriptionLabTestDto : EntityDto<long>
    {
        public int TenantId { get; set; }
        public long? PrescriptionId { get; set; }
        public long? PatientId { get; set; }
        public string PatientName { get; set; }
        public long LabReportsTypeId { get; set; }
        public string ReportTypeName { get; set; }
        public LabTestStatus TestStatus { get; set; }
        public bool IsPaid { get; set; } = true;
        public bool IsFromPackage { get; set; }
        public long? HealthPackageId { get; set; }
        public string PackageName { get; set; }
        public DateTime? CreatedDate { get; set; } = DateTime.Now;
        public List<LabReportResultItemDto> ResultItems { get; set; }
    }
}
