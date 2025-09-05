using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using EMRSystem.Appointments;
using EMRSystem.Doctors;
using EMRSystem.Invoices;
using EMRSystem.Patients;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMRSystem.AppointmentReceipt
{
    public class AppointmentReceipt : Entity<long>, IMustHaveTenant
    {
        public int TenantId { get; set; }
        public long AppointmentId { get; set; }
        public long PatientId { get; set; }
        public long DoctorId { get; set; } 
        public decimal ConsultationFee { get; set; } 
        public string ReceiptNumber { get; set; }
        public DateTime PaymentDate { get; set; } = DateTime.Now;
        public PaymentMethod PaymentMethod { get; set; }
        public InvoiceStatus Status { get; set; }
        public string? PaymentIntentId { get; set; }

        public virtual Appointment Appointment { get; set; }
        public virtual Patient Patient { get; set; }
        public virtual Doctor Doctor { get; set; }
    }
}
