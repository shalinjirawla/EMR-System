using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMRSystem.Emergency.EmergencyCase
{
    public class CreateUpdateEmergencyCaseDto : EntityDto<long>
    {
        public int TenantId { get; set; }
        public long? PatientId { get; set; }
        public string EmergencyNumber { get; set; }
        public DateTime ArrivalTime { get; set; } = DateTime.Now;
        public ModeOfArrival ModeOfArrival { get; set; } = ModeOfArrival.WalkIn;
        public EmergencySeverity Severity { get; set; } = EmergencySeverity.Stable;
        public EmergencyStatus Status { get; set; } = EmergencyStatus.PendingTriage;
        public long? DoctorId { get; set; }
        public long? NurseId { get; set; }
        public long? AdmissionsId { get; set; }
    }
}
