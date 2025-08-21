using Abp.Application.Services;
using EMRSystem.Emergency.EmergencyCase;

namespace EMRSystem.Emergency
{
    public interface IEmergencyAppService :
        IAsyncCrudAppService<
            EmergencyCaseDto,                  // DTO to return
            long,                              // Primary key
            PagedEmergencyCaseResultRequestDto, // For filtering/search
            CreateUpdateEmergencyCaseDto,      // For Create
            CreateUpdateEmergencyCaseDto       // For Update
        >
    {
    }
}
