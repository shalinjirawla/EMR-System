using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Domain.Entities;
using Abp.Domain.Repositories;
using Abp.Extensions;
using Abp.Linq.Extensions;
using AutoMapper.Internal.Mappers;
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
    public class RoomTypeMasterAppService : AsyncCrudAppService<
       RoomTypeMaster,
       RoomTypeMasterDto,
       long,
       PagedAndSortedResultRequestDto,
       CreateUpdateRoomTypeMasterDto,
       CreateUpdateRoomTypeMasterDto>,
       IRoomTypeMasterAppService
    {
        public RoomTypeMasterAppService(IRepository<RoomTypeMaster, long> repo)
            : base(repo) { }

        /* -------- Query filter (optional keyword search) -------- */
        //protected override IQueryable<RoomTypeMaster> CreateFilteredQuery(PagedAndSortedResultRequestDto input)
        //{
        //    var keyword = (input as dynamic).Keyword as string;   // safe cast
        //    return Repository.GetAllIncluding(r => r.Facilities)
        //                     .WhereIf(!keyword.IsNullOrWhiteSpace(),
        //                              x => x.TypeName.Contains(keyword));
        //}

        /* -------- Create with facilities -------- */
        public async Task<RoomTypeMasterDto> CreateWithFacilitiesAsync(CreateUpdateRoomTypeMasterDto input)
        {
            var entity = ObjectMapper.Map<RoomTypeMaster>(input);
            AddFacilityLinks(entity, input.FacilityIds);

            await Repository.InsertAsync(entity);
            await CurrentUnitOfWork.SaveChangesAsync();

            /*  ⬇️  Re‑query so navigations are populated */
            entity = await Repository.GetAll()                 // start with bare query
                         .Include(r => r.Facilities)
                         .ThenInclude(f => f.RoomFacilityMaster)
                         .FirstAsync(r => r.Id == entity.Id);

            return ObjectMapper.Map<RoomTypeMasterDto>(entity);
        }

        public async Task<ListResultDto<RoomTypeMasterDto>> GetAllRoomTypeByTenantID(int tenantId)
        {
            var query = Repository.GetAll()
                .Where(x => x.TenantId == tenantId);
            var roomtype = await query.ToListAsync();
            var mapped = ObjectMapper.Map<List<RoomTypeMasterDto>>(roomtype);
            return new ListResultDto<RoomTypeMasterDto>(mapped);
        }

        /* -------- Update with facilities -------- */
        public async Task UpdateWithFacilitiesAsync(CreateUpdateRoomTypeMasterDto input)
        {
            var entity = await Repository.GetAllIncluding(r => r.Facilities)
                                         .FirstOrDefaultAsync(r => r.Id == input.Id);
            if (entity == null)
                throw new EntityNotFoundException(typeof(RoomTypeMaster), input.Id);

            ObjectMapper.Map(input, entity);   // scalar props

            entity.Facilities.Clear();
            AddFacilityLinks(entity, input.FacilityIds);

            await Repository.UpdateAsync(entity);
            await CurrentUnitOfWork.SaveChangesAsync();
        }

        /* -------- Get for edit -------- */
        public async Task<CreateUpdateRoomTypeMasterDto> GetForEditAsync(long id)
        {
            var entity = await Repository.GetAllIncluding(r => r.Facilities)
                                         .FirstOrDefaultAsync(r => r.Id == id);
            if (entity == null)
                throw new EntityNotFoundException(typeof(RoomTypeMaster), id);

            var dto = ObjectMapper.Map<CreateUpdateRoomTypeMasterDto>(entity);
            dto.FacilityIds = entity.Facilities.Select(f => f.RoomFacilityMasterId).ToList();
            return dto;
        }

        /* -------- helper -------- */
        private static void AddFacilityLinks(RoomTypeMaster entity, IList<long> facilityIds)
        {
            if (facilityIds?.Any() != true) return;

            foreach (var fid in facilityIds.Distinct())
            {
                entity.Facilities.Add(new RoomTypeFacility
                {
                    TenantId = entity.TenantId,
                    RoomFacilityMasterId = fid
                });
            }
        }
    }
}
