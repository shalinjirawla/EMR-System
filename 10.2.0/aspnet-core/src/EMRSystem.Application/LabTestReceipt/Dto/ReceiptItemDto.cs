using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMRSystem.LabTestReceipt.Dto
{
    public class ReceiptItemDto
    {
        // common
        public bool IsPackage { get; set; }
        public DateTime? CreatedDate { get; set; }

        // package fields (when IsPackage == true)
        public long? PackageId { get; set; }
        public string PackageName { get; set; }
        public decimal? PackagePrice { get; set; }
        public List<string> PackageTests { get; set; } = new List<string>();

        // individual test fields (when IsPackage == false)
        public long? PrescriptionLabTestId { get; set; }
        public long? LabReportsTypeId { get; set; }
        public string LabReportTypeName { get; set; }
        public decimal? LabReportPrice { get; set; }

        // extra metadata
        public bool? IsPaid { get; set; }
        public int? TestStatus { get; set; }
    }
}
