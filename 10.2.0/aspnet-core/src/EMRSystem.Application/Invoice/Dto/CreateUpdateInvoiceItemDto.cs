using Abp.Application.Services.Dto;
using EMRSystem.Invoices;
using EMRSystem.IpdChargeEntry;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMRSystem.Invoice.Dto
{
    public class CreateUpdateInvoiceItemDto : EntityDto<long>
    {

        public long InvoiceId { get; set; }

        public string Description { get; set; }

        public int Quantity { get; set; } = 1;

        public decimal UnitPrice { get; set; }
        public DateTime EntryDate { get; set; } = DateTime.Now;
        public bool IsCoveredByInsurance { get; set; } = false;
        public decimal? ApprovedAmount { get; set; }
        public decimal? NotApprovedAmount { get; set; }
    }
}
