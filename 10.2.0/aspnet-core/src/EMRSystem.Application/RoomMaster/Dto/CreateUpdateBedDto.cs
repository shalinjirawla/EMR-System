using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMRSystem.RoomMaster.Dto
{
    public class CreateUpdateBedDto : EntityDto<long>
    {
        public int TenantId { get; set; }
        public string BedNumber { get; set; }

        public long RoomId { get; set; }

        public BedStatus Status { get; set; } = BedStatus.Available;
    }
}
