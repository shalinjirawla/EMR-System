using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMRSystem.ProcedureReceipts.Dto
{
    public class CreateProcedureReceiptWithIdsDto
    {
        public CreateUpdateProcedureReceiptDto Input { get; set; }
        public long[] SelectedProcedureIds { get; set; }
    }
}
