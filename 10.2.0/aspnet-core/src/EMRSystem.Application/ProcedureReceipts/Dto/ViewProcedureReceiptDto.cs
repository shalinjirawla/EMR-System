using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMRSystem.ProcedureReceipts.Dto
{
    public class ViewProcedureReceiptDto
    {
        public string PatientName { get; set; }
        public string ReceiptNumber { get; set; }
        public decimal TotalFee { get; set; }
        public DateTime PaymentDate { get; set; }
        public string PaymentMethod { get; set; }
        public string Status { get; set; }

        public List<ViewSelectedProcedureDto> SelectedProcedures { get; set; }
    }
}
