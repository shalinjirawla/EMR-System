using Abp.Application.Services.Dto;
using Abp.Runtime.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMRSystem.Insurances.Dto
{
    public class PagedInsuranceClaimDto : PagedResultRequestDto
    {
        public string Keyword { get; set; }
    }
}
