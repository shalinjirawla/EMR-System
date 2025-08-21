using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMRSystem.Emergency.EmergencyCase
{
    public class CreateUpdateEmergencyCaseDto : EntityDto<long>
    {
        public int TenantId { get; set; }
        public long? PatientId { get; set; }
        public ModeOfArrival ModeOfArrival { get; set; }
        public EmergencySeverity Severity { get; set; }
        public EmergencyStatus Status { get; set; }
        public long? DoctorId { get; set; }
        public long? NurseId { get; set; }
        public DateTime ArrivalTime { get; set; } = DateTime.Now;
    }
}
