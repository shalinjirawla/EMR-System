using Abp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMRSystem.Prescriptions
{
    public class PrescriptionItem : Entity<long>
    {
        public string MedicineName { get; set; }
        public string Dosage { get; set; } // e.g. 500mg
        public string Frequency { get; set; } // e.g. Twice a day
        public string Duration { get; set; } // e.g. 5 days
        public string Instructions { get; set; }
        public long PrescriptionId { get; set; }
        public int MedicineId { get; set; }
        public virtual Prescription Prescription { get; set; }
    }
}
