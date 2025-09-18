using Abp.Application.Services.Dto;
using Abp.Runtime.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMRSystem.Patient_Discharge.Dto
{
    public class PagedPatientDischargeResultRequestDto : PagedResultRequestDto
    {
        public string Keyword { get; set; }
    }
}
