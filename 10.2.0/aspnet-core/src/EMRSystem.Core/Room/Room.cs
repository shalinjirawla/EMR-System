using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using EMRSystem.RoomMaster;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMRSystem.Room
{
    public class Room : Entity<long>, IMustHaveTenant
    {
        public int TenantId { get; set; }

        public string RoomNumber { get; set; }
        public int Floor { get; set; }

        /* FK to master */
        public long RoomTypeMasterId { get; set; }
        public virtual RoomTypeMaster RoomTypeMaster { get; set; }
        public virtual ICollection<EMRSystem.Admission.Admission> Admissions { get; set; }


        public RoomStatus Status { get; set; }
    }

    public enum RoomStatus
    {
        Available,
        Occupied,
        Reserved,
        UnderMaintenance
    }
}
