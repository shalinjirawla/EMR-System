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
    public interface IRoomFacilityMasterAppService
        : IAsyncCrudAppService<
            RoomFacilityMasterDto, long,
            PagedAndSortedResultRequestDto,
            CreateUpdateRoomFacilityMasterDto,
            CreateUpdateRoomFacilityMasterDto>
    { }
}
