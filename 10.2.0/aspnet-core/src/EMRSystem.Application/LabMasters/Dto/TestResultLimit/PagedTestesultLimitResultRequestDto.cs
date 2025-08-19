using Abp.Application.Services.Dto;
using Abp.Runtime.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMRSystem.LabMasters.Dto.TestResultLimit
{
    public class PagedTestResultLimitResultRequestDto : PagedResultRequestDto, IShouldNormalize
    {
        public string Keyword { get; set; }
        public void Normalize()
        {
            Keyword = Keyword?.Trim();
        }
    }
}
