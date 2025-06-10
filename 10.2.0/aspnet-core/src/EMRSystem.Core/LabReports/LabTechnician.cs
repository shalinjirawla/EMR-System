using Abp.Domain.Entities;
using EMRSystem.Authorization.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMRSystem.LabReports
{
    public class LabTechnician : Entity<long>
    {
        public string FullName { get; set; }
        public string Gender { get; set; }
        public string Qualification { get; set; }
        public int YearsOfExperience { get; set; }
        public LabDepartment Department { get; set; }
        public string CertificationNumber { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public long AbpUserId { get; set; }
        public virtual User AbpUser { get; set; }
        public ICollection<LabReport> LabReports { get; set; }
    }

    public enum LabDepartment
    {
        Pathology,
        Radiology,
        Biochemistry,
        Microbiology,
        Hematology
    }
}
