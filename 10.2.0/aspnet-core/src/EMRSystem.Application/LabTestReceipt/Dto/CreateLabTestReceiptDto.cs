using EMRSystem.Invoices;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMRSystem.LabTestReceipt.Dto
{
    public class CreateLabTestReceiptDto
    {
        public int TenantId { get; set; }
        [Required]
        public long PatientId { get; set; }

        [Required]
        public LabTestSource LabTestSource { get; set; }

        public long? PrescriptionId { get; set; } // Only for OPD

        [Required]
        public List<long> SelectedTestIds { get; set; } = new List<long>();

        public List<long> SelectedPackageIds { get; set; } = new List<long>();

        [Required]
        public PaymentMethod PaymentMethod { get; set; }

        [Required]
        public decimal TotalAmount { get; set; }
    }
}
