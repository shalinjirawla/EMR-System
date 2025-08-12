using Abp.Application.Services.Dto;
using EMRSystem.LabMasters.Dto.LabTest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMRSystem.LabReportsType.Dto
{
    public class LabReportsTypeDto : EntityDto<long>
    {
        public int TenantId { get; set; }
        public string ReportType { get; set; }
        public int ReportPrice { get; set; }
        public List<LabTestDto> Tests { get; set; }

    }
}
