
using Abp.Application.Services.Dto;
using EMRSystem.LabReports;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMRSystem.LabTechnician.Dto
{
    public class LabRequestListDto : EntityDto<long>
    {
        public long PatientId { get; set; }
        public string PatientName { get; set; }
        public bool IsAdmitted { get; set; }
        public bool IsPaid { get; set; }
        public long DoctorId { get; set; }
        public string DoctorName { get; set; }
        public long PrescriptionId { get; set; }
        public long LabReportsTypeId { get; set; }
        public string LabReportTypeName { get; set; }
        public LabTestStatus TestStatus { get; set; }
    }
}
