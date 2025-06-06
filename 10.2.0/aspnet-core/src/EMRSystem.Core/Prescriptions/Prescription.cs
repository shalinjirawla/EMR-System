using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMRSystem.Prescriptions
{
    public class Prescription : FullAuditedEntity<long>, IMustHaveTenant
    {
        public int TenantId { get; set; }
        public long DoctorId { get; set; }
        public long PatientId { get; set; }
        public long AppointmentId { get; set; }
        public DateTime DateIssued { get; set; }
        public string Notes { get; set; }
        public ICollection<PrescriptionItem> Items { get; set; }
    }
}
