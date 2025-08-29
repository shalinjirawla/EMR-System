using Abp.Domain.Entities;
using EMRSystem.Prescriptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMRSystem.EmergencyProcedure
{
    public class SelectedEmergencyProcedures : Entity<long>, IMustHaveTenant
    {
        public int TenantId { get; set; }
        public long EmergencyProcedureId { get; set; }
        public EmergencyProcedure EmergencyProcedures { get; set; }
        public long PrescriptionId { get; set; }
        public virtual Prescription Prescriptions { get; set; }
    }
}
