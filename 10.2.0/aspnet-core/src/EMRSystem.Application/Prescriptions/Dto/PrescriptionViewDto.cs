using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMRSystem.Prescriptions.Dto
{
    public class PrescriptionViewDto
    {
        public long Id { get; set; }
        public string Diagnosis { get; set; }
        public string Notes { get; set; }
        public DateTime IssueDate { get; set; }
        public bool IsFollowUpRequired { get; set; }
        public DateTime? AppointmentDate { get; set; }

        // Patient Information
        public string PatientFullName { get; set; }
        public DateTime PatientDateOfBirth { get; set; }
        public string PatientGender { get; set; }
        public string PatientBloodGroup { get; set; }

        // Doctor Information
        public string DoctorFullName { get; set; }
        public string DoctorSpecialization { get; set; }
        public string DoctorRegistrationNumber { get; set; }

        // Medications
        public List<PrescriptionItemViewDto> Items { get; set; }

        // Lab Tests
        public List<PrescriptionLabTestViewDto> LabTests { get; set; }
    }
}
