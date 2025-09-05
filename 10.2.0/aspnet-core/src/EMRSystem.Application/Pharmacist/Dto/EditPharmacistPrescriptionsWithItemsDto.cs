using Abp.Application.Services.Dto;
using EMRSystem.Pharmacists;
using EMRSystem.Prescriptions.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMRSystem.Pharmacist.Dto
{
    public class EditPharmacistPrescriptionsWithItemsDto
    {
        public long? PrescriptionId { get; set; }
        public long? PatientId { get; set; }
        public string PatientName { get; set; }
        public string PharmacyNotes { get; set; }
        public DateTime? IssueDate { get; set; }
        public CollectionStatus CollectionStatus { get; set; }
        public long? PickedUpByNurse { get; set; }
        public long? PickedUpByPatient { get; set; }
        public List<PharmacistPrescriptionItemWithUnitPriceDto> PrescriptionItem { get; set; }
    }
}
