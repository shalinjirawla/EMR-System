using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMRSystem.RoomMaster
{
    public class Bed : Entity<long>, IMustHaveTenant
    {
        public int TenantId { get; set; }
        public string BedNumber { get; set; }

        public long RoomId { get; set; }
        public virtual EMRSystem.Room.Room Room { get; set; }

        public BedStatus Status { get; set; } = BedStatus.Available;
        public string Notes { get; set; }
    }

    public enum BedStatus
    {
        Available,
        Occupied,
        Reserved,
        UnderMaintenance
    }
}
