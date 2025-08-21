using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMRSystem.RoomMaster.Dto
{
    public class PagedBedResultRequestDto : PagedAndSortedResultRequestDto
    {
        public string Keyword { get; set; }
        public long? RoomId { get; set; }
        public long? RoomTypeId { get; set; }

        public BedStatus? Status { get; set; }
    }
}
