using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMRSystem.IpdChargeEntry.Dto
{
    public class IpdChargeEntryDto : EntityDto<long>
    {
        public long AdmissionId { get; set; }
        public long PatientId { get; set; }
        public string PatientName { get; set; }
        public string ChargeType { get; set; }
        public string Description { get; set; }
        public decimal Amount { get; set; }
        public DateTime EntryDate { get; set; }
        public bool IsProcessed { get; set; }
        public long? PrescriptionId { get; set; }

        public long? ReferenceId { get; set; }
    }
}
