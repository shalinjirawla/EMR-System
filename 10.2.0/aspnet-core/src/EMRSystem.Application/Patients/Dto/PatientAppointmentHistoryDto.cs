using EMRSystem.Appointments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMRSystem.Patients.Dto
{
    public class PatientAppointmentHistoryDto
    {
        public DateTime AppointmentDate { get; set; }
        public TimeSpan StartTime { get; set; }
        public bool IsFollowUp { get; set; }
        public long DoctorId { get; set; }
        public string DoctorName { get; set; }
        public long? NurseId { get; set; }
        public string NurseName { get; set; }
        public AppointmentStatus Status { get; set; }
        public string ReasonForVisit { get; set; }
    }
}
