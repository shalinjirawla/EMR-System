using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMRSystem.Prescriptions.Dto
{
    public class PrescriptionItemDto : EntityDto<long>
    {
        public string MedicineName { get; set; }
        public string Dosage { get; set; } // e.g. 500mg
        public string Frequency { get; set; } // e.g. Twice a day
        public string Duration { get; set; } // e.g. 5 days
        public string Instructions { get; set; }
        public long PrescriptionId { get; set; }
        public int MedicineId { get; set; }
        public int Qty { get; set; }
        public decimal UnitPrice { get; set; }
        public bool IsPrescribe { get; set; }
    }
}
