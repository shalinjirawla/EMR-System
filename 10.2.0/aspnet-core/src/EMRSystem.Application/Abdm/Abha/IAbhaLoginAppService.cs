using Abp.Application.Services;
using EMRSystem.Abdm.Abha.Dto;
using System.Threading.Tasks;

namespace EMRSystem.Abdm.Abha
{
    public interface IAbhaLoginAppService : IApplicationService
    {
        Task<RequestLoginOtpOutput> RequestLoginOtpAsync(RequestLoginOtpInput input);
        Task<VerifyLoginOtpOutput> VerifyLoginOtpAsync(VerifyLoginOtpInput input);
        Task<LinkProfileOutput> FetchAndLinkProfileAsync(FetchProfileInput input);
        Task<GetAbhaCardOutput> GetAbhaCardAsync(GetProfileMediaInput input);
        Task<GetAbhaQrCodeOutput> GetAbhaQrCodeAsync(GetProfileMediaInput input);
    }
}
