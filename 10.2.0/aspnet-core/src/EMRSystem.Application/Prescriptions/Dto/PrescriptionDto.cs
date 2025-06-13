using Abp.Application.Services.Dto;
using EMRSystem.Doctor.Dto;
using EMRSystem.Patients;
using EMRSystem.Patients.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMRSystem.Prescriptions.Dto
{
    public class PrescriptionDto : EntityDto<long>
    {
        public int TenantId { get; set; }

        public string Diagnosis { get; set; }
        public string Notes { get; set; }
        public DateTime IssueDate { get; set; } = DateTime.Now;
        public bool IsFollowUpRequired { get; set; }
        public DoctorDto Doctor { get; set; }
        public PatientDto Patient { get; set; }
        public List<PrescriptionItemDto> Items { get; set; }
    }
}
