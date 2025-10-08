using Abp.Application.Services;
using Abp.Application.Services.Dto;
using EMRSystem.EmergencyProcedure.Dto;
using EMRSystem.ProcedureReceipts.Dto;
using System.Threading.Tasks;

namespace EMRSystem.ProcedureReceipts
{
    public interface IProcedureReceiptAppService :
        IAsyncCrudAppService<ProcedureReceiptDto, long, PagedProcedureReceiptDto, CreateUpdateProcedureReceiptDto, CreateUpdateProcedureReceiptDto>
    {
        // Normal create (cash/payment counter case)
        Task<ProcedureReceiptDto> CreateProcedureReceiptAsync(CreateProcedureReceiptWithIdsDto dto);

        // Stripe webhook success ke baad
        Task<ProcedureReceiptDto> CreateProcedureReceiptFromStripeAsync(CreateUpdateProcedureReceiptDto input, long[] selectedProcedureIds, string paymentIntentId);

        // Stripe checkout session banane ke liye
        Task<string> CreateStripeCheckoutSession(CreateProcedureReceiptWithIdsDto dto);
    }
}
