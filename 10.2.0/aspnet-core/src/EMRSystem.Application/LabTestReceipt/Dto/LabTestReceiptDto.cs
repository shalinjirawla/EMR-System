using Abp.Application.Services.Dto;
using EMRSystem.Invoices;
using EMRSystem.Patients.Dto;
using EMRSystem.PrescriptionLabTest.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMRSystem.LabTestReceipt.Dto
{
    public class LabTestReceiptDto : EntityDto<long>
    {
        public int TenantId { get; set; }
        public long PatientId { get; set; }
        public PatientDto Patient { get; set; }
        public decimal TotalFee { get; set; }
        public string? PaymentIntentId { get; set; }
        public string ReceiptNumber { get; set; }
        public DateTime PaymentDate { get; set; }
        public PaymentMethod PaymentMethod { get; set; }
        public InvoiceStatus Status { get; set; }
        public LabTestSource Source { get; set; }
        public List<PrescriptionLabTestDto> PrescriptionLabTests { get; set; }
    }
}
