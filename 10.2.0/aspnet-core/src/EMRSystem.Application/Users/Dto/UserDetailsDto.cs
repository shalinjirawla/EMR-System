using EMRSystem.Doctor.Dto;
using EMRSystem.Nurse.Dto;
using EMRSystem.Patients.Dto;
using EMRSystem.Roles.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMRSystem.Users.Dto
{
    public class UserDetailsDto
    {
        public long Id { get; set; }
        public int TenantId { get; set; }
        public string UserName { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string EmailAddress { get; set; }
        public List<RoleDto> Roles { get; set; }
        public List<PatientDto> Patients { get; set; }
        public List<DoctorDto> Doctors { get; set; }
        public List<NurseDto> Nurses { get; set; }
        public List<EMRSystem.LabReports.LabTechnician> LabTechnicians { get; set; }
    }
}
