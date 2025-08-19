using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMRSystem.LabTestReceipt.Dto
{
    //public class ViewLabTestReceiptDto
    //{
    //    public int? TenantId { get; set; }

    //    public long LabReportTypeId { get; set; }

    //    public long PatientId { get; set; }

    //    public decimal LabTestFee { get; set; }

    //    public string ReceiptNumber { get; set; }

    //    public DateTime PaymentDate { get; set; }

    //    public string PaymentMethod { get; set; }

    //    public string Status { get; set; }

    //    public long PrescriptionLabTestId { get; set; }

    //    public string LabReportName { get; set; }

    //    public string PatientName { get; set; }

    //    public DateTime LabReportDate { get; set; }
    //}
    public class ViewLabTestReceiptDto : LabTestReceiptDto
    {
        public string PatientName { get; set; }
        public string PatientPhone { get; set; }
        public string PaymentMethodDisplay => PaymentMethod.ToString();
        public string StatusDisplay => Status.ToString();
    }
}
