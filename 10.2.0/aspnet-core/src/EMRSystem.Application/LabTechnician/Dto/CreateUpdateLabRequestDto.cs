using Abp.Application.Services.Dto;
using EMRSystem.LabReports;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMRSystem.LabTechnician.Dto
{
    public class CreateUpdateLabRequestDto : EntityDto<long>
    {
        public int TenantId { get; set; }
        public long? PatientId { get; set; }
        public long? PrescriptionId { get; set; }
        public long LabReportsTypeId { get; set; }
        public LabTestStatus TestStatus { get; set; }
    }
}
