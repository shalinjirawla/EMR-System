using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMRSystem.AppointmentReceipt.Dto
{
    public class CreateAppointmentReceiptDto
    {
        public long AppointmentId { get; set; }
        public string PaymentMethod { get; set; }
    }
}
