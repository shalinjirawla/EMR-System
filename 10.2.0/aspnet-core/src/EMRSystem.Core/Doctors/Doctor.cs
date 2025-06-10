using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using EMRSystem.Authorization.Users;
using EMRSystem.Patients;
using EMRSystem.Prescriptions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMRSystem.Doctors
{
    public class Doctor : Entity<long>
    {
        public string FullName { get; set; }
        public string Gender { get; set; }
        public string Specialization { get; set; }
        public string Qualification { get; set; }
        public int YearsOfExperience { get; set; }
        public string Department { get; set; }
        public string RegistrationNumber { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public long AbpUserId { get; set; }
        public virtual User AbpUser { get; set; }
        public ICollection<Patient> Patients { get; set; }
        public ICollection<Prescription> Prescriptions { get; set; }
    }
}
