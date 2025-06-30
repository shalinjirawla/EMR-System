using Abp.Application.Services.Dto;
using Abp.Domain.Entities;
using EMRSystem.LabReports;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMRSystem.LabReport.Dto
{
    public class LabOrderListDto : EntityDto<long>
    {
        public string PatientName { get; set; }
        public string Gender { get; set; }
        public DateTime AppointmentDate { get; set; }
        public string DoctorName { get; set; }
        public string LabReportTypeName { get; set; }
        public LabTestStatus TestStatus { get; set; }
    }
}
