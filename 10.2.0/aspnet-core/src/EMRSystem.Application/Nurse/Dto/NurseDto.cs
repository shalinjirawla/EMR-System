using Abp.Application.Services.Dto;
using EMRSystem.Authorization.Users;
using EMRSystem.Users.Dto;
using EMRSystem.Vitals;
using EMRSystem.Vitals.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMRSystem.Nurse.Dto
{
    public class NurseDto : EntityDto<long>
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
        public DateTime? DateOfBirth { get; set; }
        public UserDto AbpUser { get; set; }
        //public List<VitalDto> Vitals { get; set; }
    }
}
