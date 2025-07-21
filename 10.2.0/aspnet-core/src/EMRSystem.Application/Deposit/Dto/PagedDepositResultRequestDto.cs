using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMRSystem.Deposit.Dto
{
    public class PagedDepositResultRequestDto : PagedResultRequestDto
    {
        public string Keyword { get; set; }
    }
}
