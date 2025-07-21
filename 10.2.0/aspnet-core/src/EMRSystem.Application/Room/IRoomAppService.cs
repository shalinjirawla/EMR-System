using Abp.Application.Services;
using EMRSystem.Room.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMRSystem.Room
{
    public interface IRoomAppService : IAsyncCrudAppService<
         RoomDto, long,
         PagedRoomResultRequestDto,
         CreateUpdateRoomDto,
         CreateUpdateRoomDto>
    {
        //Task<RoomDto> CreateRoomWithFacilitiesAsync(CreateUpdateRoomDto input);
        //Task UpdateRoomWithFacilitiesAsync(CreateUpdateRoomDto input);
        Task<List<RoomDto>> GetAvailableRoomsAsync(int tenantId);
        Task<List<RoomDto>> CreateBulkRoomsAsync(List<CreateUpdateRoomDto> input);

        Task<CreateUpdateRoomDto> GetRoomDetailsByIdAsync(long id);
    }
}
