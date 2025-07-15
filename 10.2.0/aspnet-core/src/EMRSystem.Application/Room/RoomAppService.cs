using Abp.Application.Services;
using Abp.Domain.Entities;
using Abp.Domain.Repositories;
using Abp.Extensions;
using Abp.Linq.Extensions;
using AutoMapper.Internal.Mappers;
using EMRSystem.Room.Dto;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMRSystem.Room
{
    public class RoomAppService : AsyncCrudAppService<
        Room, RoomDto, long,
        PagedRoomResultRequestDto,
        CreateUpdateRoomDto,
        CreateUpdateRoomDto>,
        IRoomAppService
    {
        public RoomAppService(IRepository<Room, long> repository)
            : base(repository) { }
        protected override IQueryable<Room> CreateFilteredQuery(PagedRoomResultRequestDto input)
        {
            return Repository
                .GetAllIncluding(r => r.RoomTypeMaster)
                .WhereIf(!input.Keyword.IsNullOrWhiteSpace(), r =>
                      r.RoomNumber.Contains(input.Keyword))
                .WhereIf(input.RoomTypeMasterId.HasValue,
                      r => r.RoomTypeMasterId == input.RoomTypeMasterId)
                .WhereIf(input.Status.HasValue,
                      r => r.Status == input.Status);
        }
        public async Task<RoomDto> CreateRoomWithFacilitiesAsync(CreateUpdateRoomDto input)
        {
            var room = ObjectMapper.Map<Room>(input);
            await Repository.InsertAsync(room);
            await CurrentUnitOfWork.SaveChangesAsync();
            return ObjectMapper.Map<RoomDto>(room);
        }

        public async Task UpdateRoomWithFacilitiesAsync(CreateUpdateRoomDto input)
        {
            var room = await Repository.GetAllIncluding()
                                       .FirstOrDefaultAsync(r => r.Id == input.Id);
            if (room == null)
                throw new EntityNotFoundException(typeof(Room), input.Id);

            ObjectMapper.Map(input, room);      

            await Repository.UpdateAsync(room);
            await CurrentUnitOfWork.SaveChangesAsync();
        }

        public async Task<CreateUpdateRoomDto> GetRoomDetailsByIdAsync(long id)
        {
            var room = await Repository.GetAllIncluding()
                                       .FirstOrDefaultAsync(r => r.Id == id);
            if (room == null)
                throw new EntityNotFoundException(typeof(Room), id);

            var dto = ObjectMapper.Map<CreateUpdateRoomDto>(room);
            return dto;
        }

    }
}
