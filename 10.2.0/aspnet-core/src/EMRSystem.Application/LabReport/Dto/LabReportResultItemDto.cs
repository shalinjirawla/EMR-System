using Abp.Application.Services.Dto;
using EMRSystem.PrescriptionLabTest.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMRSystem.LabReport.Dto
{
    public class LabReportResultItemDto : EntityDto<long>
    {
        public string Test { get; set; }
        public string Result { get; set; }
        public decimal MinValue { get; set; }
        public decimal MaxValue { get; set; }
        public string Unit { get; set; }
        public string Flag { get; set; }
        public PrescriptionLabTestDto PrescriptionLabTest { get; set; }
    }
}
