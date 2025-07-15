using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMRSystem.RoomMaster
{
    public class RoomTypeFacility : Entity<long>, IMustHaveTenant
    {
        public int TenantId { get; set; }

        public long RoomTypeMasterId { get; set; }
        public virtual RoomTypeMaster RoomTypeMaster { get; set; }

        public long RoomFacilityMasterId { get; set; }
        public virtual RoomFacilityMaster RoomFacilityMaster { get; set; }
    }
}
