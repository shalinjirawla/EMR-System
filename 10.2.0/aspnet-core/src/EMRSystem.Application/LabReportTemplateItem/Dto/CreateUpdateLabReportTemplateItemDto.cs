using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMRSystem.LabReportTemplateItem.Dto
{
    public class CreateUpdateLabReportTemplateItemDto : EntityDto<long>
    {
        public int TenantId { get; set; }
        public string Test { get; set; }
        public decimal? Result { get; set; }
        public string? Unit { get; set; }
        public decimal? MinValue { get; set; }
        public decimal? MaxValue { get; set; }
        public long LabReportsTypeId { get; set; }
    }
}
