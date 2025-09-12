using Abp.Application.Services.Dto;
using EMRSystem.Invoices;
using EMRSystem.MedicineOrder;
using EMRSystem.Nurse.Dto;
using EMRSystem.Pharmacists;
using EMRSystem.Prescriptions.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMRSystem.Pharmacist.Dto
{
    public class PharmacistPrescriptionsDto : EntityDto<long>
    {
        public int TenantId { get; set; }
        public long? PrescriptionId { get; set; }
        public string PatientName { get; set; }
        public string DoctorName { get; set; }
        public DateTime? IssueDate { get; set; }
        public string PharmacyNotes { get; set; }
        public CollectionStatus CollectionStatus { get; set; }
        //public string? PaymentIntentId { get; set; }
        //public PaymentMethod? PaymentMethod { get; set; }
        public long? PickedUpByNurse { get; set; }
        public long? PickedUpByPatient { get; set; }
        public bool IsPaid { get; set; }
        public decimal GrandTotal { get; set; }
        public List<PharmacistPrescriptionItemWithUnitPriceDto> prescriptionItems { get; set; }
    }
}
