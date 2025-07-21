using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMRSystem.DoctorMaster.Dto
{
    public class PagedDoctorMasterResultRequestDto : PagedResultRequestDto
    {
        public string Keyword { get; set; }
    }
}
