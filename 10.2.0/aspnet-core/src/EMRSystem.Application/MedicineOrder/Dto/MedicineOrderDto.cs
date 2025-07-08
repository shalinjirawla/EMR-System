using Abp.Application.Services.Dto;
using EMRSystem.Nurse.Dto;
using EMRSystem.Patients.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMRSystem.MedicineOrder.Dto
{
    public class MedicineOrderDto : EntityDto<long>
    {
        public int TenantId { get; set; }

        public long NurseId { get; set; }
        public NurseDto Nurse { get; set; }

        public long? PatientId { get; set; }
        public PatientDto Patient { get; set; }

        public DateTime OrderDate { get; set; }

        public string Status { get; set; }

        public string Priority { get; set; }

        public List<MedicineOrderItemDto> Items { get; set; }
    }
}
