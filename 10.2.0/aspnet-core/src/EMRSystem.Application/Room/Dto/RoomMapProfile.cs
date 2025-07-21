    using AutoMapper;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    namespace EMRSystem.Room.Dto
    {
    public class RoomMapProfile : Profile
    {
        public RoomMapProfile()
        {
            CreateMap<Room, RoomDto>()
                .ForMember(d => d.RoomTypeName,
                           o => o.MapFrom(s => s.RoomTypeMaster.TypeName));

            CreateMap<CreateUpdateRoomDto, Room>()
                .ForMember(d => d.RoomTypeMasterId,
                           o => o.MapFrom(s => s.RoomTypeMasterId))
                .ReverseMap();                      
        }
    }


}
