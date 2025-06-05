using Abp.Application.Services;
using EMRSystem.Authorization.Accounts.Dto;
using System.Threading.Tasks;

namespace EMRSystem.Authorization.Accounts;

public interface IAccountAppService : IApplicationService
{
    Task<IsTenantAvailableOutput> IsTenantAvailable(IsTenantAvailableInput input);

    Task<RegisterOutput> Register(RegisterInput input);
}
