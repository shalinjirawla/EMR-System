using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMRSystem.ProcedureReceipts.Dto
{
    public class ViewSelectedProcedureDto
    {
        public long Id { get; set; }
        public long EmergencyProcedureId { get; set; }
        public string EmergencyProcedureName { get; set; }
        public decimal Price { get; set; }
        public bool IsPaid { get; set; }
        public string Status { get; set; }
        public long? PrescriptionId { get; set; }
    }
}
