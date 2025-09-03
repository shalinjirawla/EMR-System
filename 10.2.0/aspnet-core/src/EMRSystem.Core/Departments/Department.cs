using Abp.Domain.Entities;
using EMRSystem.Appointments;
using EMRSystem.Doctors;
using EMRSystem.LabReports;
using EMRSystem.Prescriptions;
using EMRSystem.Visits;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMRSystem.Departments
{
    public class Department : Entity<long>, IMustHaveTenant
    {
        public int TenantId { get; set; }
        public string DepartmentName { get; set; }
        public DepartmentType? DepartmentType { get; set; }
        public bool IsActive { get; set; } = true;
        public virtual ICollection<Doctor> Doctors { get; set; }
        public ICollection<Prescription> Prescriptions { get; set; }
        public virtual ICollection<LabTechnician> LabTechnicians { get; set; }
        public virtual ICollection<Appointment> Appointments { get; set; }


    }
    public enum DepartmentType
    {
        Doctor,
        LabTechnician
    }
}
