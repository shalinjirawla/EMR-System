using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMRSystem.Insurances.Dto
{
    public class InsuranceMasterDto : EntityDto<long>
    {
        public int TenantId { get; set; }
        public string InsuranceName { get; set; }

        public bool CoversRoomCharge { get; set; }
        public bool CoversDoctorVisit { get; set; }
        public bool CoversLabTests { get; set; }
        public bool CoversProcedures { get; set; }
        public bool CoversMedicines { get; set; }
        public bool IsActive { get; set; }
    }
}
