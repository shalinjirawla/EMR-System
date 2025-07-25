using EMRSystem.AppointmentReceipt.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMRSystem.Appointments.Dto
{
    public class AppointmentCreationResultDto
    {
        public bool IsStripeRedirect { get; set; }
        public string StripeSessionUrl { get; set; }
        public AppointmentReceiptDto Receipt { get; set; }
        public string Message { get; set; }
    }
}
