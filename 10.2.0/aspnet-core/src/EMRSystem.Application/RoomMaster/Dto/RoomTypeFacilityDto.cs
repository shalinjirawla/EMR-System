using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMRSystem.RoomMaster.Dto
{
    public class RoomTypeFacilityDto : EntityDto<long>
    {
        public int TenantId { get; set; }

        public long RoomTypeMasterId { get; set; }
        public string RoomTypeName { get; set; }   // convenience

        public long RoomFacilityMasterId { get; set; }
        public string FacilityName { get; set; }  // convenience
    }
}
