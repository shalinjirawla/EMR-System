using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMRSystem.LabReportsType.Dto
{
    public class CreateUpdateLabReportTypeDto : EntityDto<long>
    {
        public int TenantId { get; set; }
        public string ReportType { get; set; }
    }
}
