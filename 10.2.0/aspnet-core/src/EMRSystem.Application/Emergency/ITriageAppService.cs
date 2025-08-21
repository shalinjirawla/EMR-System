using Abp.Application.Services;
using EMRSystem.Emergency.Triage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMRSystem.Emergency
{
    public interface ITriageAppService :
    IAsyncCrudAppService<
        TriageDto,                  // DTO to return
        long,                       // Primary key
        PagedTriageResultRequestDto,// Paged request DTO
        CreateUpdateTriageDto,
        CreateUpdateTriageDto>      // Create/Update DTO
    {
    }
}
