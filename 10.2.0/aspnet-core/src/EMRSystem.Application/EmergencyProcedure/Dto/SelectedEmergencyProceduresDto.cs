using Abp.Application.Services.Dto;
using Abp.Domain.Entities;
using EMRSystem.Prescriptions;
using EMRSystem.Prescriptions.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMRSystem.EmergencyProcedure.Dto
{
    public class SelectedEmergencyProceduresDto:EntityDto<long>
    {
        public int TenantId { get; set; }
        public EmergencyProcedureDto EmergencyProcedures { get; set; }
        //public  PrescriptionDto Prescriptions { get; set; }
        public bool IsPaid { get; set; }
        public EmergencyProcedureStatus Status { get; set; }
        public string ProcedureName { get; set; }
        public string PatientName { get; set; }
    }
}
