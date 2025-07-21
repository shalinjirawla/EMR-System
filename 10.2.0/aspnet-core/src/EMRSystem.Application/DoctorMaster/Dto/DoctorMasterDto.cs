using Abp.Application.Services.Dto;
using EMRSystem.Doctor.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMRSystem.DoctorMaster.Dto
{
    public class DoctorMasterDto : EntityDto<long>
    {
        public int TenantId { get; set; }
        public long DoctorId { get; set; }
        public DoctorDto Doctor { get; set; }
        public decimal Fee { get; set; }
    }
}
