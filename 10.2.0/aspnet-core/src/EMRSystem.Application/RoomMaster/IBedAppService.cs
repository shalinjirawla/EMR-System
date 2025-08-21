using Abp.Application.Services;
using EMRSystem.RoomMaster.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMRSystem.RoomMaster
{
    public interface IBedAppService : IAsyncCrudAppService<
        BedDto, long,
        PagedBedResultRequestDto,
        CreateUpdateBedDto,
        CreateUpdateBedDto>
    {
        Task<List<BedDto>> GetBedsAsync(int tenantId);
        Task<CreateUpdateBedDto> GetBedDetailsByIdAsync(long id);
    }
}
