using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMRSystem.Patient_Discharge.Dto
{
    public class ViewPrescriptionSummary
    {
        public long Id { get; set; }
        public string Diagnosis { get; set; }
        public DateTime IssueDate { get; set; }
        public string PrescribedBy { get; set; }
    }
}
