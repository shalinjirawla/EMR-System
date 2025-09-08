using Abp.Application.Services.Dto;
using EMRSystem.MedicineOrder;
using EMRSystem.Pharmacists;
using EMRSystem.Prescriptions.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMRSystem.Pharmacist.Dto
{
    public class CreateUpdatePharmacistPrescriptionsDto : EntityDto<long>
    {
        public int TenantId { get; set; }
        public long? PrescriptionId { get; set; }
        public long? PickedUpByNurse { get; set; }
        public long? PickedUpByPatient { get; set; }
        public DateTime? IssueDate { get; set; }
        public string PharmacyNotes { get; set; }
        public CollectionStatus CollectionStatus { get; set; }
        public decimal GrandTotal { get; set; }
    }
    public class CreatePharmacistPrescriptionsWithItemDto
    {
        public CreateUpdatePharmacistPrescriptionsDto pharmacistPrescriptionsDto { get; set; }
        public List<PharmacistPrescriptionItemWithUnitPriceDto> pharmacistPrescriptionsListOfItem { get; set; }
    }
}
