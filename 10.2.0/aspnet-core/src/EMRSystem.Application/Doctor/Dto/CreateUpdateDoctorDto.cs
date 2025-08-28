using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using EMRSystem.Authorization.Users;
using EMRSystem.Users.Dto;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMRSystem.Doctor.Dto
{
    public class CreateUpdateDoctorDto : EntityDto<long>
    {
        public int TenantId { get; set; }

        public string FullName { get; set; }
        public string Gender { get; set; }
        public string Specialization { get; set; }
        public string Qualification { get; set; }
        public int YearsOfExperience { get; set; }
        public long? DepartmentId { get; set; }
        public string RegistrationNumber { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public long AbpUserId { get; set; }
        public bool isEmergencyDoctor { get; set; }
    }
}
