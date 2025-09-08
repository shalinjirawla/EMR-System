using EMRSystem.LabReport.Dto;
using EMRSystem.Pharmacists;
using EMRSystem.Prescriptions.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMRSystem.Pharmacist.Dto
{
    public class ViewPharmacistPrescriptionsDto
    {
        public string TenantName { get; set; }
        public long PharmacistPrescriptionId { get; set; }
        public long PrescriptionId { get; set; }
        public long PatientId { get; set; }
        public string PatientName { get; set; }
        public DateTime? PatientDateOfBirth { get; set; }
        public string Gender { get; set; }
        public string DoctorName { get; set; }
        public DateTime IssueDate { get; set; }
        public string DoctorRegistrationNumber { get; set; }
        public CollectionStatus CollectionStatus { get; set; }
        public bool IsPaid { get; set; }
        public string PharmacyNotes { get; set; }
        public long? PickedUpByNurseId { get; set; }
        public long? PickedUpByPatientId { get; set; }
        public string PickedUpByNurse { get; set; }
        public string PickedUpByPatient { get; set; }
        public decimal GrandTotal { get; set; }
        public List<PrescriptionItemDto> PrescriptionItems { get; set; }
    }
}
