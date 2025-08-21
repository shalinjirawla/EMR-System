using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMRSystem.RoomMaster.Dto
{
    public class BedMapProfile : Profile
    {
        public BedMapProfile()
        {
            CreateMap<Bed, BedDto>()
                .ForMember(dest => dest.RoomTypeName, opt => opt.MapFrom(src => src.Room.RoomTypeMaster.TypeName))
                .ForMember(d => d.RoomNumber,
                           o => o.MapFrom(s => s.Room.RoomNumber));

            CreateMap<CreateUpdateBedDto, Bed>()
                .ForMember(d => d.RoomId,
                           o => o.MapFrom(s => s.RoomId))
                .ReverseMap();
        }
    }
}
