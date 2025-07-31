using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMRSystem.Prescriptions.Dto
{
    public class PrescriptionLabTestViewDto
    {
        public string ReportTypeName { get; set; }
        public string TestStatus { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
