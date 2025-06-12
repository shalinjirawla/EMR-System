using Abp.Application.Services.Dto;
using Abp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMRSystem.Appointments.Dto
{
    public class CreateUpdateAppointmentDto : EntityDto<long>
    {
        public DateTime AppointmentDate { get; set; }
        public string AppointmentTimeSlot { get; set; }
        public string ReasonForVisit { get; set; }
        public AppointmentStatus Status { get; set; }
        public bool IsFollowUp { get; set; }
        public long PatientId { get; set; }
        public long DoctorId { get; set; }
        public long? NurseId { get; set; }
    }
}
