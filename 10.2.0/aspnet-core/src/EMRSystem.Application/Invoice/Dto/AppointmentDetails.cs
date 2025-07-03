using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMRSystem.Invoice.Dto
{
    public class AppointmentDetails
    {
        public long AppointmentId { get; set; }
        public DateTime AppointmentDate { get; set; }
        public string PatientName { get; set; }
        public string DoctorName { get; set; }
        public decimal ConsultationFee { get; set; }
    }

}
