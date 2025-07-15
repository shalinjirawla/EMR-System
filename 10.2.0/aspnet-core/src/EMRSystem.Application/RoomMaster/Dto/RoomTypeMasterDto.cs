using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMRSystem.RoomMaster.Dto
{
    public class RoomTypeMasterDto : EntityDto<long>
    {
        public int TenantId { get; set; }
        public string TypeName { get; set; }
        public string Description { get; set; }
        public decimal DefaultPricePerDay { get; set; }

        public List<long> FacilityIds { get; set; }
        public List<RoomFacilityMasterDto> Facilities { get; set; }
    }
}
