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
    public class RoomTypeMaster : Entity<long>, IMustHaveTenant
    {
        public int TenantId { get; set; }

        [Required, StringLength(100)]
        public string TypeName { get; set; }       // e.g. Deluxe Room

        [StringLength(250)]
        public string Description { get; set; }

        public decimal DefaultPricePerDay { get; set; }

        public ICollection<RoomTypeFacility> Facilities { get; set; } = new List<RoomTypeFacility>();
    }
}
