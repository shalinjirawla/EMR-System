using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMRSystem.Prescriptions.Dto
{
    public class PrescriptionItemViewDto
    {
        public string MedicineName { get; set; }
        public string Dosage { get; set; }
        public string Frequency { get; set; }
        public string Duration { get; set; }
        public string Instructions { get; set; }
        public int MedicineId { get; set; }
    }
}
