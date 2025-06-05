using Abp.Application.Services;
using EMRSystem.MultiTenancy.Dto;

namespace EMRSystem.MultiTenancy;

public interface ITenantAppService : IAsyncCrudAppService<TenantDto, int, PagedTenantResultRequestDto, CreateTenantDto, TenantDto>
{
}

