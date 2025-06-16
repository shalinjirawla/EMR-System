using Abp.Application.Services.Dto;
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
        public string AppointmentTimeSlot { get; set; }
        public string ReasonForVisit { get; set; }
        public string PatientName { get; set; }
        public string DoctorName { get; set; }
        public AppointmentStatus Status { get; set; }
        public bool IsFollowUp { get; set; }
        public PatientDto Patient { get; set; }
        public DoctorDto Doctor { get; set; }
        public NurseDto Nurse { get; set; }
        public List<PrescriptionDto> Prescriptions { get; set; }
    }
}
