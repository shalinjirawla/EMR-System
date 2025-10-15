using Abp.Application.Services;
using Abp.Application.Services.Dto;
using EMRSystem.Insurances.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMRSystem.Insurances
{
    public interface IInsuranceClaimAppService :
        IAsyncCrudAppService<InsuranceClaimDto, long, PagedInsuranceClaimDto,
            CreateUpdateInsuranceClaimDto, CreateUpdateInsuranceClaimDto>
    {
        // extra APIs useful for billing flow
        Task<InsuranceClaimDto> GetByInvoiceIdAsync(long invoiceId);
        Task SubmitClaimAsync(long claimId); // mark as Submitted
        Task MarkAsPaidAsync(EntityDto<long> input, decimal paidAmount);
    }
}
