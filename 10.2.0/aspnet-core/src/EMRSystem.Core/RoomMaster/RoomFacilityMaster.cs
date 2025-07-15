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
    public class RoomFacilityMaster : Entity<long>, IMustHaveTenant
    {
        public int TenantId { get; set; }

        [Required, StringLength(100)]
        public string FacilityName { get; set; } 
    }
}
