using Abp.Application.Services;
using EMRSystem.Deposit.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMRSystem.Deposit
{
    public interface IDepositAppService : IAsyncCrudAppService<
        DepositDto, long, PagedDepositResultRequestDto, CreateUpdateDepositDto, CreateUpdateDepositDto>
    { }
}
