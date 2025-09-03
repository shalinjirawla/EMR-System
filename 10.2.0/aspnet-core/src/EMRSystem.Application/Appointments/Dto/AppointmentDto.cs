using Abp.Application.Services.Dto;
using EMRSystem.Departments.Dto;
using EMRSystem.Doctor.Dto;
using EMRSystem.Nurse.Dto;
using EMRSystem.Patients.Dto;
using EMRSystem.Prescriptions;
using EMRSystem.Prescriptions.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMRSystem.Appointments.Dto
{
    public class AppointmentDto : EntityDto<long>
    {
        public int TenantId { get; set; }
        public DateTime AppointmentDate { get; set; }
        //public TimeSpan StartTime { get; set; }   // New
        //public TimeSpan EndTime { get; set; }     // New
        public string ReasonForVisit { get; set; }
        public AppointmentStatus Status { get; set; }
        //public PatientType PatientType { get; set; }
        public bool IsFollowUp { get; set; }
        public long AppointmentTypeId { get; set; }
        public bool IsPaid { get; set; }
        public long DepartmentId { get; set; }
        public DepartmentDto Department { get; set; }

        public virtual EMRSystem.AppointmentType.AppointmentType AppointmentType { get; set; }
        public PatientDto Patient { get; set; }
        public DoctorDto Doctor { get; set; }
        //public NurseDto Nurse { get; set; }
        public List<PrescriptionDto> Prescriptions { get; set; }
    }
}
