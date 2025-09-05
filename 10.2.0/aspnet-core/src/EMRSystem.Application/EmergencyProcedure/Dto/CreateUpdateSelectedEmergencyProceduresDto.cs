using Abp.Application.Services.Dto;
using EMRSystem.Prescriptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMRSystem.EmergencyProcedure.Dto
{
    public class CreateUpdateSelectedEmergencyProceduresDto : EntityDto<long>
    {
        public int TenantId { get; set; }
        public long EmergencyProcedureId { get; set; }
        public long PrescriptionId { get; set; }
        public bool IsPaid { get; set; }
        public EmergencyProcedureStatus Status { get; set; }
    }
}
