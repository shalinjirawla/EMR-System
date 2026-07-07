using Abp.Application.Services;
using EMRSystem.Abdm.Abha.Dto;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EMRSystem.Abdm.Abha
{
    public interface IAbhaEnrollmentAppService : IApplicationService
    {
        Task<RequestAadhaarOtpOutput> RequestAadhaarOtpAsync(RequestAadhaarOtpInput input);
        Task<object> VerifyAadhaarOtpAsync(VerifyAadhaarOtpInput input);
        Task<List<string>> GetAddressSuggestionsAsync(SuggestAddressInput input);
        Task<CreateAbhaAddressOutput> CreateAbhaAddressAsync(CreateAbhaAddressInput input);
    }
}
