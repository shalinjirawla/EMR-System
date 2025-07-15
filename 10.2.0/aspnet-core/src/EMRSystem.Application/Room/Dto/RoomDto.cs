using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMRSystem.Room.Dto
{
    public class RoomDto : EntityDto<long>
    {
        public int TenantId { get; set; }
        public string RoomNumber { get; set; }
        public int Floor { get; set; }

        public long RoomTypeMasterId { get; set; }
        public string RoomTypeName { get; set; }
        public RoomStatus Status { get; set; }
    }

}
