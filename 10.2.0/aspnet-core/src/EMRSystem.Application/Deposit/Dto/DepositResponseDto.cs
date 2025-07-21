using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMRSystem.Deposit.Dto
{
    public class DepositResponseDto
    {
        public DepositDto? Deposit { get; set; }
        public string? StripeRedirectUrl { get; set; }
    }

}
