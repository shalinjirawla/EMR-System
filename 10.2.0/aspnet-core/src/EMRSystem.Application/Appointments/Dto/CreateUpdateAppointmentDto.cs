using Abp.Application.Services.Dto;
using Abp.Domain.Entities;
using EMRSystem.Invoices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMRSystem.Appointments.Dto
{
    public class CreateUpdateAppointmentDto : EntityDto<long>
    {
        public int TenantId { get; set; }
        public DateTime AppointmentDate { get; set; }
        //public TimeSpan StartTime { get; set; }   // New
        //public TimeSpan EndTime { get; set; }     // New
        public string ReasonForVisit { get; set; }
        public AppointmentStatus Status { get; set; }
        //public PatientType PatientType { get; set; }

        public bool IsFollowUp { get; set; }
        public bool IsPaid { get; set; }

        public PaymentMethod PaymentMethod { get; set; }
        public long PatientId { get; set; }
        public long DoctorId { get; set; }
        //public long? NurseId { get; set; }
        public long? AppointmentTypeId { get; set; }
    }
}
