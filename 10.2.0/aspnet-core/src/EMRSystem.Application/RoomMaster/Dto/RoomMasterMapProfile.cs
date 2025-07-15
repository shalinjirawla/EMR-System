using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMRSystem.RoomMaster.Dto
{
    public class RoomMasterMapProfile : Profile
    {
        public RoomMasterMapProfile()
        {
            /* ───────── FacilityMaster <‑‑> DTO ───────── */
            CreateMap<RoomMaster.RoomFacilityMaster, RoomFacilityMasterDto>().ReverseMap();
            CreateMap<CreateUpdateRoomFacilityMasterDto, RoomMaster.RoomFacilityMaster>().ReverseMap();

            /* ───────── RoomTypeMaster ➜ DTO ───────── */
            CreateMap<RoomMaster.RoomTypeMaster, RoomTypeMasterDto>()
                .ForMember(d => d.FacilityIds,
                           o => o.MapFrom(s => s.Facilities
                                                 .Select(f => f.RoomFacilityMasterId)))
                .ForMember(d => d.Facilities,
                           o => o.MapFrom(s => s.Facilities
                                                 .Select(f => new RoomFacilityMasterDto
                                                 {
                                                     Id = f.RoomFacilityMasterId,
                                                     FacilityName = f.RoomFacilityMaster.FacilityName,
                                                     TenantId = f.TenantId
                                                 })));

            /* ───────── Create / Update RoomType ───────── */
            CreateMap<CreateUpdateRoomTypeMasterDto, RoomMaster.RoomTypeMaster>()
                .ForMember(d => d.Facilities, o => o.Ignore());

            /* ───────── Linking table ➜ DTO (optional) ───────── */
            CreateMap<RoomMaster.RoomTypeFacility, RoomTypeFacilityDto>()
                .ForMember(d => d.RoomTypeName,
                           o => o.MapFrom(s => s.RoomTypeMaster.TypeName))
                .ForMember(d => d.FacilityName,
                           o => o.MapFrom(s => s.RoomFacilityMaster.FacilityName));
        }
    }
}
