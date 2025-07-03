using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMRSystem.Invoice.Dto
{
    public class LabTestDetailDto
    {
        public long PrescriptionLabTestId { get; set; }
        public string TestName { get; set; }
        public decimal Price { get; set; }
    }
}
