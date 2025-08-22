using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMRSystem.Emergency.EmergencyCase
{
    public class EmergencyCaseDto : EntityDto<long>
    {
        public int TenantId { get; set; }
        public long? PatientId { get; set; }
        public string PatientName { get; set; } // For display, mapped in your service
        public DateTime? PatientDateOfBirth { get; set; } // For display

        public string EmergencyNumber { get; set; }
        public DateTime ArrivalTime { get; set; }
        public ModeOfArrival ModeOfArrival { get; set; }

        // This should reflect the latest triage severity
        public EmergencySeverity Severity { get; set; }
        public EmergencyStatus Status { get; set; }

        public long? DoctorId { get; set; }
        public string DoctorName { get; set; } // For display

        public long? NurseId { get; set; }
        public string NurseName { get; set; } // For display
        public long? AdmissionsId { get; set; }
    }


    public class EmergencyCaseLookupDto : EntityDto<long>
    {
        public string EmergencyNumber { get; set; }
        public string PatientName { get; set; }
        public EmergencyStatus Status { get; set; }
    }
}
