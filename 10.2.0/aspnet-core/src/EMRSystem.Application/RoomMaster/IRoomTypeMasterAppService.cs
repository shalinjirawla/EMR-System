using Abp.Application.Services;
using Abp.Application.Services.Dto;
using EMRSystem.RoomMaster.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMRSystem.RoomMaster
{
    public interface IRoomTypeMasterAppService
        : IAsyncCrudAppService<
            RoomTypeMasterDto, long,
            PagedAndSortedResultRequestDto,
            CreateUpdateRoomTypeMasterDto,
            CreateUpdateRoomTypeMasterDto>
    {
        Task<RoomTypeMasterDto> CreateWithFacilitiesAsync(CreateUpdateRoomTypeMasterDto input);
        Task UpdateWithFacilitiesAsync(CreateUpdateRoomTypeMasterDto input);
        Task<CreateUpdateRoomTypeMasterDto> GetForEditAsync(long id);
    }
}
