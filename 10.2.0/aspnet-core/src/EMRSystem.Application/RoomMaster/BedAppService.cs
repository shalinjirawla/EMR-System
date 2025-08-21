using Abp.Application.Services;
using Abp.Domain.Entities;
using Abp.Domain.Repositories;
using Abp.EntityFrameworkCore;
using Abp.Extensions;
using Abp.Linq.Extensions;
using Abp.UI;
using AutoMapper.Internal.Mappers;
using EMRSystem.EntityFrameworkCore;
using EMRSystem.RoomMaster.Dto;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Text;
using System.Threading.Tasks;

namespace EMRSystem.RoomMaster
{
    public class BedAppService : AsyncCrudAppService<
         Bed, BedDto, long,
         PagedBedResultRequestDto,
         CreateUpdateBedDto,
         CreateUpdateBedDto>,
         IBedAppService
    {
        private readonly IDbContextProvider<EMRSystemDbContext> _dbContextProvider;

        public BedAppService(IDbContextProvider<EMRSystemDbContext> dbContextProvider,
                             IRepository<Bed, long> repository)
            : base(repository)
        {
            _dbContextProvider = dbContextProvider;
        }

        protected override IQueryable<Bed> CreateFilteredQuery(PagedBedResultRequestDto input)
        {
            var query = Repository
                .GetAllIncluding(b => b.Room, b => b.Room.RoomTypeMaster) // RoomTypeMaster include
                .WhereIf(!input.Keyword.IsNullOrWhiteSpace(),
                         b => b.BedNumber.Contains(input.Keyword) ||
                              b.Room.RoomNumber.Contains(input.Keyword) ||
                              b.Room.RoomTypeMaster.TypeName.Contains(input.Keyword)) // filter RoomTypeName
                .WhereIf(input.RoomId.HasValue,
                         b => b.RoomId == input.RoomId)
                .WhereIf(input.Status.HasValue,
                         b => b.Status == input.Status)
                .WhereIf(input.RoomTypeId.HasValue,
                         b => b.Room.RoomTypeMasterId == input.RoomTypeId); // filter RoomType by Id

            return query;
        }


        protected override IQueryable<Bed> ApplySorting(IQueryable<Bed> query, PagedBedResultRequestDto input)
        {
            if (!string.IsNullOrWhiteSpace(input.Sorting))
            {
                var sorting = input.Sorting;

                if (sorting.Contains("roomNumber", System.StringComparison.OrdinalIgnoreCase))
                    sorting = sorting.Replace("roomNumber", "Room.RoomNumber", System.StringComparison.OrdinalIgnoreCase);

                if (sorting.Contains("roomId", System.StringComparison.OrdinalIgnoreCase))
                    sorting = sorting.Replace("roomId", "Room.Id", System.StringComparison.OrdinalIgnoreCase);

                return DynamicQueryableExtensions.OrderBy(query, sorting);
            }

            return base.ApplySorting(query, input);
        }

        public async Task<List<BedDto>> GetBedsAsync(int tenantId)
        {
            var beds = await Repository
                .GetAllIncluding(b => b.Room)
                .Where(b => b.TenantId == tenantId)
                .OrderBy(b => b.Room.RoomNumber)
                .ThenBy(b => b.BedNumber)
                .ToListAsync();

            return ObjectMapper.Map<List<BedDto>>(beds);
        }

        public async Task<CreateUpdateBedDto> GetBedDetailsByIdAsync(long id)
        {
            var bed = await Repository.GetAllIncluding(b => b.Room)
                                      .FirstOrDefaultAsync(b => b.Id == id);

            if (bed == null)
                throw new EntityNotFoundException(typeof(Bed), id);

            return ObjectMapper.Map<CreateUpdateBedDto>(bed);
        }
        public async Task<List<BedDto>> CreateBulkBedsAsync(List<CreateUpdateBedDto> inputs)
        {
            var dbContext = await _dbContextProvider.GetDbContextAsync();

            // Collect input values
            var roomIds = inputs.Select(i => i.RoomId).Distinct().ToList();
            var bedNumbers = inputs.Select(i => i.BedNumber).ToList();

            // Fetch all existing beds including Room + RoomType
            var existingBeds = await dbContext.Beds
                .Include(b => b.Room)
                    .ThenInclude(r => r.RoomTypeMaster) // 👈 RoomType ke liye
                .Where(b => roomIds.Contains(b.RoomId) && bedNumbers.Contains(b.BedNumber))
                .ToListAsync();

            if (existingBeds.Any())
            {
                var duplicate = string.Join(", ",
                    existingBeds.Select(b =>
                        $"Bed '{b.BedNumber}' in Room '{b.Room.RoomNumber}' (Type: {b.Room.RoomTypeMaster.TypeName})"
                    )
                );

                throw new UserFriendlyException($"Duplicate beds found: {duplicate}");
            }

            // Map and save
            var entities = ObjectMapper.Map<List<Bed>>(inputs);
            dbContext.Beds.AddRange(entities);
            await dbContext.SaveChangesAsync();

            return ObjectMapper.Map<List<BedDto>>(entities);
        }

        public async Task<List<BedDto>> GetAvailableBedsByRoomAsync(int tenantId, long roomId)
        {
            var beds = await Repository
                .GetAllIncluding(b => b.Room)
                .Where(b => b.TenantId == tenantId
                            && b.RoomId == roomId
                            && b.Status == BedStatus.Available)
                .OrderBy(b => b.BedNumber)
                .ToListAsync();

            return ObjectMapper.Map<List<BedDto>>(beds);
        }



    }
}
