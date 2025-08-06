using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMRSystem.LabReportsType.Dto
{
    public class LabReportDetailDto
    {
        public string ReportName { get; set; }
        public decimal ReportPrice { get; set; }
        public List<LabTestItemDto> LabTests { get; set; }
    }

}
