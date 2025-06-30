using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMRSystem.LabReport.Dto
{
    public class ViewLabReportDto : EntityDto<long>
    {
        public string TenantName { get; set; }
        public string TestName { get; set; }
        public long PrescriptionLabTestId { get; set; }
        public long PatientId { get; set; }
        public string PatientName { get; set; }
        public string PatientDateOfBirth { get; set; }
        public string Gender { get; set; }
        public string DoctorName { get; set; }
        public DateTime RecordedDate { get; set; }
        public string DoctorRegistrationNumber { get; set; }
        public List<LabReportResultItemDto> LabReportResultItem { get; set; }
    }
}
