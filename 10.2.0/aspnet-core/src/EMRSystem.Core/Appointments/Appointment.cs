using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;

namespace EMRSystem.Appointments
{
    public class Appointment : FullAuditedEntity<long>, IMustHaveTenant
    {
        public int TenantId { get; set; }
        public long PatientId { get; set; }
        public long DoctorId { get; set; }
        public long? ScheduledByUserId { get; set; }
        public DateTime AppointmentDate { get; set; }
        public string ReasonForVisit { get; set; }
        public AppointmentStatus Status { get; set; }
    }
    public enum AppointmentStatus
    {
        Scheduled,
        Completed,
        Cancelled
    }
}
