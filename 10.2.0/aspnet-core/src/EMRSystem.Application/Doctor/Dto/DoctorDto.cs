using Abp.Application.Services.Dto;
using EMRSystem.Authorization.Users;
using EMRSystem.Patients;
using EMRSystem.Patients.Dto;
using EMRSystem.Prescriptions;
using EMRSystem.Prescriptions.Dto;
using EMRSystem.Users.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMRSystem.Doctor.Dto
{
    public class DoctorDto : EntityDto<long>
    {
        public string FullName { get; set; }
        public string Gender { get; set; }
        public string Specialization { get; set; }
        public string Qualification { get; set; }
        public int YearsOfExperience { get; set; }
        public string Department { get; set; }
        public string RegistrationNumber { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public virtual UserDto AbpUser { get; set; }
        public List<PatientDto> Patients { get; set; }
        public List<PrescriptionDto> Prescriptions { get; set; }
    }
}
