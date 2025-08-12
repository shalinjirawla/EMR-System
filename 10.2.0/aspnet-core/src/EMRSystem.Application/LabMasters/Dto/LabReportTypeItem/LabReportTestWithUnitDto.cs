using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMRSystem.LabMasters.Dto.LabReportTypeItem
{
    public class LabReportTestWithUnitDto : EntityDto<long>
    {
        public long LabReportTypeId { get; set; }
        public long LabTestId { get; set; }
        public string LabTestName { get; set; }

        public long? MeasureUnitId { get; set; }
        public string? MeasureUnitName { get; set; }

        public bool IsActive { get; set; }
    }
}
