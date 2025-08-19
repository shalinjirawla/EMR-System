using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMRSystem.LabMasters.Dto.TestResultLimit
{
    public class CreateUpdateTestResultLimitDto : EntityDto<long>
    {
        public int TenantId { get; set; }
        public long LabTestId { get; set; }
        public float? MinRange { get; set; }
        public float? MaxRange { get; set; }
    }
}
