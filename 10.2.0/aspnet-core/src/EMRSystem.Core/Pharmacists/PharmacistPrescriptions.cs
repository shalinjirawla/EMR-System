using Abp.Domain.Entities;
using EMRSystem.Doctors;
using EMRSystem.MedicineOrder;
using EMRSystem.Nurses;
using EMRSystem.Patients;
using EMRSystem.Prescriptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMRSystem.Pharmacists
{
    public class PharmacistPrescriptions : Entity<long>, IMustHaveTenant
    {
        public int TenantId { get; set; }
        public long? PrescriptionId { get; set; }
        public virtual Prescription Prescriptions { get; set; }
        public DateTime? IssueDate { get; set; }
        public OrderStatus Order_Status { get; set; }

        //for OPD Patient
        public string PharmacyNotes { get; set; }
        public CollectionStatus CollectionStatus { get; set; }
        public long? PickedUpBy { get; set; }
        public Nurse Nurse { get; set; }
        public virtual ICollection<PharmacistPrescriptionsItem> PharmacistPrescriptionsItem { get; set; }
    }

    public enum CollectionStatus
    {
        NotPickedUp,
        PickedUp
    }
}
