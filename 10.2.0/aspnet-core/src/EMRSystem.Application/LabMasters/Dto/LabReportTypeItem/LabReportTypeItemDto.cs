using Abp.Application.Services.Dto;
using EMRSystem.LabMasters.Dto.LabTest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMRSystem.LabMasters.Dto.LabReportTypeItem
{
    public class LabReportTypeItemDto : EntityDto<long>
    {
        public int TenantId { get; set; }

        public long LabReportTypeId { get; set; }

        public long LabTestId { get; set; }

        public LabTestDto LabTest { get; set; }

        public bool IsActive { get; set; }
    }
}
