using EMRSystem.Patients.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMRSystem.LabTestReceipt.Dto
{
    public class LabTestReceiptDisplayDto
    {
        public long Id { get; set; }
        public int TenantId { get; set; }
        public long PatientId { get; set; }
        public PatientDto Patient { get; set; }

        public decimal TotalFee { get; set; }
        public string ReceiptNumber { get; set; }
        public DateTime PaymentDate { get; set; }
        public string PaymentMethod { get; set; }
        public string Status { get; set; }
        public  string Source { get; set; }

        /// <summary>
        /// Prepared items for display. Each item is either a package (IsPackage = true)
        /// or an individual test (IsPackage = false).
        /// </summary>
        public List<ReceiptItemDto> Items { get; set; } = new List<ReceiptItemDto>();
    }
}
