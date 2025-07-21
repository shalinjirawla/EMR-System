using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMRSystem.DoctorMaster.Dto
{
    public class CreateUpdateDoctorMasterDto : EntityDto<long>
    {
        public int TenantId { get; set; }
        public long DoctorId { get; set; }
        public decimal Fee { get; set; }
    }
}
