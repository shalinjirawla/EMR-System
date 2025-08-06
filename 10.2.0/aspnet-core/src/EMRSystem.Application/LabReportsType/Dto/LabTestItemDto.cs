using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMRSystem.LabReportsType.Dto
{
    public class LabTestItemDto
    {
        public long LabTestId { get; set; }
        public string LabTestName { get; set; }
        public string MeasureUnitName { get; set; }
        public float? MinRange { get; set; }
        public float? MaxRange { get; set; }
    }

}
