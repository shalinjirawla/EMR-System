using Abp.Application.Services.Dto;
using EMRSystem.Doctors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMRSystem.Doctor.Dto
{
    public class ConsultationRequestsDto : EntityDto<long>
    {
        public int TenantId { get; set; }
        public string PatientName { get; set; }
        public long? PrescriptionId { get; set; }
        public string RequestingDoctorName { get; set; }
        public long? RequestingDoctorId { get; set; }
        public string RequestedSpecialistName { get; set; }
        public long? RequestedSpecialistId { get; set; }
        public Status Status { get; set; }
        public string Notes { get; set; }
        public string AdviceResponse { get; set; }
    }
}
