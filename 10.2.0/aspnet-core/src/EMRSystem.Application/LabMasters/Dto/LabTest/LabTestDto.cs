using Abp.Application.Services.Dto;
using EMRSystem.LabMasters.Dto.MeasureUnit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMRSystem.LabMasters.Dto.LabTest
{
    public class LabTestDto : EntityDto<long>
    {
        public int TenantId { get; set; }
        public string Name { get; set; }
        public long MeasureUnitId { get; set; }
        public MeasureUnitDto MeasureUnit { get; set; }
        public bool IsActive { get; set; }
    }
}
