using Abp.Application.Services.Dto;
using EMRSystem.Invoices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMRSystem.LabTestReceipt.Dto
{
    public class UpdateLabTestReceiptDto : EntityDto<long>
    {
        public InvoiceStatus Status { get; set; }
        public PaymentMethod? PaymentMethod { get; set; }
    }
}
