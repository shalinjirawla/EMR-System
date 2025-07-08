using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMRSystem.Invoice.Dto
{
    public class InvoiceDetailsDto
    {
        public long AppointmentId { get; set; }
        public DateTime AppointmentDate { get; set; }
        public string PatientName { get; set; }
        public string DoctorName { get; set; }
        public decimal ConsultationFee { get; set; } = 300;
        public List<LabTestDetailDto> LabTests { get; set; } = new();
        public List<MedicineDetailDto> Medicines { get; set; } = new();
        public decimal SubTotal { get; set; }
        public decimal GstAmount { get; set; }
        public decimal TotalAmount { get; set; }
    }
}
