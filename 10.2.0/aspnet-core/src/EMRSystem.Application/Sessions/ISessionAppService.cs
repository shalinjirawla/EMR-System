using Abp.Application.Services;
using EMRSystem.Sessions.Dto;
using System.Threading.Tasks;

namespace EMRSystem.Sessions;

public interface ISessionAppService : IApplicationService
{
    Task<GetCurrentLoginInformationsOutput> GetCurrentLoginInformations();
}
