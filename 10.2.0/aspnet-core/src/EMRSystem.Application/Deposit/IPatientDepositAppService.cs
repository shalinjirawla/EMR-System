using Abp.Application.Services;
using Abp.Application.Services.Dto;
using EMRSystem.Deposit.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMRSystem.Deposit
{
    public interface IPatientDepositAppService :
        IAsyncCrudAppService<PatientDepositDto, long, PagedAndSortedResultRequestDto, CreateUpdatePatientDepositDto>
    {
    }
}
