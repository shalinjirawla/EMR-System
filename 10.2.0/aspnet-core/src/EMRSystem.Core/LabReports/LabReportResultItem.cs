using Abp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMRSystem.LabReports
{
    public class LabReportResultItem : Entity<long>
    {
        public string Test { get; set; }
        public decimal Result { get; set; }
        public decimal MinValue { get; set; }
        public decimal MaxValue { get; set; }
        public string Unit { get; set; }
        public string Flag { get; set; }
        public long PrescriptionLabTestId { get; set; }
        public virtual PrescriptionLabTest PrescriptionLabTest { get; set; }
    }
}
