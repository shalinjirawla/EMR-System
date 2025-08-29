using EMRSystem.Prescriptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMRSystem.EmergencyProcedure.Dto
{
    public class CreateUpdateSelectedEmergencyProceduresDto
    {
        public int TenantId { get; set; }
        public long EmergencyProcedureId { get; set; }
        public long PrescriptionId { get; set; }
    }
}
