using Abp.Domain.Entities;
using EMRSystem.LabReportsTypes;
using EMRSystem.Prescriptions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMRSystem.PrescriptionLabTests
{
    public class PrescriptionLabTest : Entity<long>, IMustHaveTenant
    {
        public int TenantId { get; set; }

        public long PrescriptionId { get; set; }
        public virtual Prescription Prescription { get; set; }

        public long LabReportsTypeId { get; set; }
        public virtual LabReportsType LabReportsType { get; set; }
    }

   
}
