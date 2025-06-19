using Abp.Application.Services.Dto;
using EMRSystem.Users.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMRSystem.Pharmacist.Dto
{
    public class PharmacistDto : EntityDto<long>
    {
        public int TenantId { get; set; }

        public string FullName { get; set; }

        public string Gender { get; set; }

        public string Qualification { get; set; }

        public DateTime? DateOfBirth { get; set; }

        public string LicenseNumber { get; set; }

        public DateTime LicenseExpiryDate { get; set; }

        public UserDto AbpUser { get; set; }
    }
}
