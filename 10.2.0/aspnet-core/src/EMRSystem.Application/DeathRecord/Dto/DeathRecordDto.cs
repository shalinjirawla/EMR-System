using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMRSystem.DeathRecord.Dto
{
    public class DeathRecordDto : EntityDto<long>
    {
        public int? TenantId { get; set; }
        public long PatientId { get; set; }
        public string PatientName { get; set; }
        public DateTime DeathDate { get; set; }
        public DateTime DeathTime { get; set; }
        public long? DoctorId { get; set; }
        public string? DoctorName { get; set; }
        public long? NurseId { get; set; }
        public string? NurseName { get; set; }
        public string? CauseOfDeath { get; set; }
        public bool IsPostMortemDone { get; set; }
        public string? Notes { get; set; }
    }
}
