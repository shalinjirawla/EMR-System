using Abp.Application.Services.Dto;
using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using EMRSystem.Authorization.Users;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMRSystem.Pharmacists

{
    public class Pharmacist : Entity<long>, IMustHaveTenant
    {
        public int TenantId { get; set; }
        public string FullName { get; set; }
        public string Gender { get; set; }
        public string Qualification { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string LicenseNumber { get; set; }
        public DateTime LicenseExpiryDate { get; set; }
        public long AbpUserId { get; set; }
        public virtual User AbpUser { get; set; }
    }
}
