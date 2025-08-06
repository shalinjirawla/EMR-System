using Abp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMRSystem.LabMasters
{
    public class TestResultLimit : Entity<long>, IMustHaveTenant
    {
        public int TenantId { get; set; }

        public long LabTestId { get; set; }

        public float? MinRange { get; set; }

        public float? MaxRange { get; set; }

        [ForeignKey(nameof(LabTestId))]
        public virtual LabTest LabTest { get; set; }
    }
}
