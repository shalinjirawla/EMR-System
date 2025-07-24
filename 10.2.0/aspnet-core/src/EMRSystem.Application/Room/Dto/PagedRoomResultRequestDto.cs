using Abp.Application.Services.Dto;
using Abp.Runtime.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMRSystem.Room.Dto
{
    public class PagedRoomResultRequestDto : PagedAndSortedResultRequestDto, IShouldNormalize
    {
        public string Keyword { get; set; }
        public long? RoomTypeMasterId { get; set; }
        public RoomStatus? Status { get; set; }

        public void Normalize()
        {
            if (!string.IsNullOrEmpty(Sorting))
            {
                Sorting = Sorting.ToLowerInvariant();

                if (Sorting.Contains("roomnumber"))
                {
                    Sorting = Sorting.Replace("roomnumber", "RoomNumber");
                }
                else if (Sorting.Contains("roomtypemaster.TypeName"))
                {
                    Sorting = Sorting.Replace("roomtypemaster.name", "RoomTypeMaster.TypeName");
                }
                else if (Sorting.Contains("status"))
                {
                    Sorting = Sorting.Replace("status", "Status");
                }
            }
            else
            {
                Sorting = "RoomNumber";
            }

            Keyword = Keyword?.Trim();
        }
    }
}
