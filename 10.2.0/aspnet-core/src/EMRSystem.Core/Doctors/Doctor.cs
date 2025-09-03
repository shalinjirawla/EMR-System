using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using EMRSystem.Appointments;
using EMRSystem.Authorization.Users;
using EMRSystem.Emergency.EmergencyCase;
using EMRSystem.Patients;
using EMRSystem.Prescriptions;
using EMRSystem.Visits;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMRSystem.Doctors
{
    public class Doctor : Entity<long>, IMustHaveTenant
    {
        public int TenantId { get; set; }
        public string FullName { get; set; }
        public string Gender { get; set; }
        public string Specialization { get; set; }
        public string Qualification { get; set; }
        public int YearsOfExperience { get; set; }
        public string RegistrationNumber { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public long AbpUserId { get; set; }
        public long? DepartmentId { get; set; }
        public virtual User AbpUser { get; set; }
        public virtual Departments.Department Department { get; set; }
        //public ICollection<Patient> Patients { get; set; }
        public ICollection<Prescription> Prescriptions { get; set; }
        public ICollection<Prescription> SpecialistPrescriptions { get; set; }
        public virtual ICollection<Appointment> Appointments { get; set; }
        public virtual ICollection<EMRSystem.Admission.Admission> Admissions { get; set; }
        public virtual ICollection<EmergencyCase> EmergencyCases { get; set; }
        public ICollection<Visit> Visits { get; set; }
        public bool isEmergencyDoctor { get; set; }
        public virtual ICollection<EMRSystem.Doctors.ConsultationRequests> RequestingDoctor_Consultation_Requests { get; set; }
        public virtual ICollection<EMRSystem.Doctors.ConsultationRequests> RequestedSpecialist_Consultation_Requests { get; set; }

    }
}
