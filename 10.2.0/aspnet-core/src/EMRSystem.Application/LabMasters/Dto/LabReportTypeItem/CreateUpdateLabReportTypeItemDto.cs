using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMRSystem.LabMasters.Dto.LabReportTypeItem
{
    public class CreateUpdateLabReportTypeItemDto : EntityDto<long>
    {
        public int TenantId { get; set; }

        public long LabReportTypeId { get; set; }

        public long LabTestId { get; set; }

        public bool IsActive { get; set; }
    }
}
