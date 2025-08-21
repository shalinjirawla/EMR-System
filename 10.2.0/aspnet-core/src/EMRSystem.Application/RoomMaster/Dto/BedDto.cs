using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMRSystem.RoomMaster.Dto
{
    public class BedDto : EntityDto<long>
    {
        public int TenantId { get; set; }
        public string BedNumber { get; set; }
        public string RoomTypeName { get; set; }

        public long RoomId { get; set; }
        public string RoomNumber { get; set; }   // 👈 Navigation property
        public BedStatus Status { get; set; }
    }
}
