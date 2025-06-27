using Abp.Application.Services.Dto;
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
        public long PrescriptionId { get; set; }
        public long LabReportsTypeId { get; set; }
        public string ReportTypeName { get; set; }  
    }
}
