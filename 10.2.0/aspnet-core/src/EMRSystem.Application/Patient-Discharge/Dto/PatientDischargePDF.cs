using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMRSystem.Patient_Discharge.Dto
{
    public class PatientDischargePDF
    {
        //Hospital & Patient Info
        public string HospitalName { get; set; }
        public long PatientId { get; set; }
        public string PatientName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Gender { get; set; }
        public string Address { get; set; }
        public string BloodGroup { get; set; }
        public string EmergencyContactName { get; set; }
        public string EmergencyContactNumber { get; set; }

        public DateTime AdmissionDate { get; set; }
        public DateTime DischargeDate { get; set; }
        public string RoomNumber { get; set; }
        public string BedNumber { get; set; }

        //Attending Doctors / Consultants

    }
}
