using Abp.Application.Services.Dto;
using Abp.Runtime.Validation;
using EMRSystem.Invoices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMRSystem.Invoice.Dto
{
    public class PagedInvoiceResultRequestDto : PagedResultRequestDto, IShouldNormalize
    {
        public string Keyword { get; set; }
        //public InvoiceStatus? Status { get; set; }
        //public PaymentMethod? PaymentMethod { get; set; }
        //public DateTime? FromDate { get; set; }
        //public DateTime? ToDate { get; set; }
        public string Sorting { get; set; }

        public void Normalize()
        {
            if (!string.IsNullOrEmpty(Sorting))
            {
                Sorting = Sorting.ToLowerInvariant();
                if (Sorting.Contains("patientname"))
                {
                    Sorting = Sorting.Replace("patientname", "Patient.FullName");
                }
                else if (Sorting.Contains("invoicedate"))
                {
                    Sorting = Sorting.Replace("invoicedate", "InvoiceDate");
                }
                else if (Sorting.Contains("totalamount"))
                {
                    Sorting = Sorting.Replace("totalamount", "TotalAmount");
                }
                // Add more mappings as needed
            }
            else
            {
                Sorting = "InvoiceDate DESC";
            }

            Keyword = Keyword?.Trim();
        }
    }
}
