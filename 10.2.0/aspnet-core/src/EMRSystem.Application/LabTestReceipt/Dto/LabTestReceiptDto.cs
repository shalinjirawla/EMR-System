using Abp.Application.Services.Dto;
using EMRSystem.Invoices;
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

        public long LabReportTypeId { get; set; }

        public long PatientId { get; set; }

        public decimal LabTestFee { get; set; }

        public string ReceiptNumber { get; set; }

        public DateTime PaymentDate { get; set; }

        public PaymentMethod PaymentMethod { get; set; }

        public InvoiceStatus Status { get; set; }
        public string LabReportTypeName { get; set; }

        public string PatientName { get; set; }
    }
}
