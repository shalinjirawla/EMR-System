using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Domain.Repositories;
using EMRSystem.Deposit.Dto;
using EMRSystem.Emergency.EmergencyCase;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMRSystem.Deposit
{
    public class PatientDepositAppService :
        AsyncCrudAppService<PatientDeposit, PatientDepositDto, long, PagedAndSortedResultRequestDto, CreateUpdatePatientDepositDto>,
        IPatientDepositAppService
    {
        public PatientDepositAppService(IRepository<PatientDeposit, long> repository) : base(repository)
        {

        }
        protected override IQueryable<PatientDeposit> CreateFilteredQuery(PagedAndSortedResultRequestDto input)
        {
            return Repository.GetAll()
                .Include(x => x.Patient);
        }
    }
}
