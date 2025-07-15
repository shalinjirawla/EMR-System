using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Domain.Repositories;
using EMRSystem.LabReportsType.Dto;
using EMRSystem.RoomMaster.Dto;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMRSystem.RoomMaster
{
    public class RoomFacilityMasterAppService : AsyncCrudAppService<
        RoomFacilityMaster,
        RoomFacilityMasterDto,
        long,
        PagedAndSortedResultRequestDto,
        CreateUpdateRoomFacilityMasterDto,
        CreateUpdateRoomFacilityMasterDto>,
        IRoomFacilityMasterAppService
    {
        public RoomFacilityMasterAppService(IRepository<RoomFacilityMaster, long> repo)
            : base(repo) { }

        public async Task<ListResultDto<RoomFacilityMasterDto>> GetAllRoomFacilityByTenantID(int tenantId)
        {
            var query = Repository.GetAll()
                .Where(x => x.TenantId == tenantId);
            var roomfacility = await query.ToListAsync();
            var mapped = ObjectMapper.Map<List<RoomFacilityMasterDto>>(roomfacility);
            return new ListResultDto<RoomFacilityMasterDto>(mapped);
        }
    }
}
