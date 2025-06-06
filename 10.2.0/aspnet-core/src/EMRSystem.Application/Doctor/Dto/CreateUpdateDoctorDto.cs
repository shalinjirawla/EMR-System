using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using EMRSystem.Authorization.Users;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMRSystem.Doctor.Dto
{
    [AutoMapTo(typeof(EMRSystem.Doctors.Doctor))]
    public class CreateUpdateDoctorDto : EntityDto<long>
    {
        public int TenantId { get; set; }
        [Required]
        public string FullName { get; set; }
        [EmailAddress]
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Gender { get; set; }
        public string Specialization { get; set; }
        public string Qualification { get; set; }
        public int YearsOfExperience { get; set; }
        public string Department { get; set; }
        public string RegistrationNumber { get; set; }
        public bool IsActive { get; set; }
        //public long UserId { get; set; }
    }
}
