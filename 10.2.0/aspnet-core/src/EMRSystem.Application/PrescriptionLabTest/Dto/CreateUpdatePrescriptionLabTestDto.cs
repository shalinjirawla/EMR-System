using Abp.Application.Services.Dto;
using EMRSystem.Invoices;
using EMRSystem.LabReport.Dto;
using EMRSystem.LabReports;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMRSystem.PrescriptionLabTest.Dto
{
    public class CreateUpdatePrescriptionLabTestDto : EntityDto<long>
    {
        public int TenantId { get; set; }
        public long? PrescriptionId { get; set; }
        public long? PatientId { get; set; }
        public long LabReportsTypeId { get; set; }
        public LabTestStatus TestStatus { get; set; }
        public bool IsPaid { get; set; } = true;
        public DateTime? CreatedDate { get; set; } = DateTime.Now;
        public PaymentMethod? PaymentMethod { get; set; }
        public List<LabReportResultItemDto> ResultItems { get; set; }
    }
}
