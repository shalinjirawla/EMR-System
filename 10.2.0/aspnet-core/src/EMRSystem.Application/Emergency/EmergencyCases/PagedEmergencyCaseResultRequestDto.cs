using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMRSystem.Emergency.EmergencyCase
{
    public class PagedEmergencyCaseResultRequestDto : PagedResultRequestDto
    {
        public string Keyword { get; set; }
    }
}
