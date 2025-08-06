using EMRSystem.LabTestReceipt.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMRSystem.PrescriptionLabTest.Dto
{
    public class LabTestCreationResultDto
    {
        public bool IsStripeRedirect { get; set; }
        public string StripeSessionUrl { get; set; }
        public LabTestReceiptDto Receipt { get; set; }
        public string Message { get; set; }
    }
}
