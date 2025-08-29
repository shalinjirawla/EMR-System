using EMRSystem.Prescriptions;
using EMRSystem.Prescriptions.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMRSystem.EmergencyProcedure.Dto
{
    public class SelectedEmergencyProceduresDto
    {
        public EmergencyProcedureDto EmergencyProcedures { get; set; }
        public  PrescriptionDto Prescriptions { get; set; }
    }
}
