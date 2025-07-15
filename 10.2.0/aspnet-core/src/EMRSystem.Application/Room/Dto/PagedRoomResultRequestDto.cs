using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMRSystem.Room.Dto
{
    public class PagedRoomResultRequestDto : PagedAndSortedResultRequestDto
    {
        public string Keyword { get; set; }
        public long? RoomTypeMasterId { get; set; }
        public RoomStatus? Status { get; set; }
    }
}
