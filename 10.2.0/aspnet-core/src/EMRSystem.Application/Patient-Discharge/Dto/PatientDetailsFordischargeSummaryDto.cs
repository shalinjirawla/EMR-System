using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMRSystem.Patient_Discharge.Dto
{
    public class PatientDetailsFordischargeSummaryDto
    {
        public long PatientId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UserName { get; set; }
        public string EmailAddress { get; set; }
        public string Gender { get; set; }
        public DateTime Dob { get; set; }
        public int Age { get; set; }
        public string BloodGroup { get; set; }
        public string MobileNumber { get; set; }
        public string EmergencyNumber { get; set; }
        public string EmergencyPersonName { get; set; }
        public string Address { get; set; }
        public DateTime AdmissionDateTime { get; set; }
    }
}
