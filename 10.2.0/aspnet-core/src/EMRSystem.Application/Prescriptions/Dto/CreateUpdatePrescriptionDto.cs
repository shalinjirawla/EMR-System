using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMRSystem.Prescriptions.Dto
{
    public class CreateUpdatePrescriptionDto : EntityDto<long>
    {
        public int TenantId { get; set; }
        public string Diagnosis { get; set; }
        public string Notes { get; set; }
        public DateTime IssueDate { get; set; } = DateTime.Now;
        public bool IsFollowUpRequired { get; set; }
        public long AppointmentId { get; set; }
        public long DoctorId { get; set; }
        public long PatientId { get; set; }
        public List<PrescriptionItemDto> Items { get; set; }
    }
}
