using Abp.Domain.Entities;
using EMRSystem.Doctors;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMRSystem.DoctorMaster
{
    public class DoctorMaster : Entity<long>, IMustHaveTenant
    {
        public int TenantId { get; set; }
        public long DoctorId { get; set; }
        public virtual Doctor Doctor { get; set; }
        public decimal Fee { get; set; }
    }
}
