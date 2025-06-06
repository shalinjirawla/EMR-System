using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMRSystem.Nurses
{
    public class Nurse : FullAuditedEntity<long>, IMustHaveTenant
    {
        public int TenantId { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Gender { get; set; }
        public string ShiftTiming { get; set; }
        public string Department { get; set; }
        public string Qualification { get; set; }
        public int YearsOfExperience { get; set; }
        public long UserId { get; set; }
    }
}
