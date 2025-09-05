using Abp.Application.Services.Dto;
using EMRSystem.Invoices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMRSystem.AppointmentReceipt.Dto
{
    // AppointmentReceiptDto.cs
    public class AppointmentReceiptDto : EntityDto<long>
    {
        public int TenantId { get; set; }
        public long AppointmentId { get; set; }
        public long PatientId { get; set; }
        public long DoctorId { get; set; } 
        public decimal ConsultationFee { get; set; }
        public string ReceiptNumber { get; set; }
        public DateTime PaymentDate { get; set; }
        public string PaymentMethod { get; set; }
        public string PatientName { get; set; }
        public string DoctorName { get; set; }
        public string Status { get; set; }
        public string AppointmentDate { get; set; }
        public string? PaymentIntentId { get; set; }

    }
}
