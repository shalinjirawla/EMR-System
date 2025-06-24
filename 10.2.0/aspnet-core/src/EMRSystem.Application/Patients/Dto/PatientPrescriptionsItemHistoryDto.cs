using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMRSystem.Patients.Dto
{
    public class PatientPrescriptionsItemHistoryDto
    {
        public string MedicineName { get; set; }
        public string Dosage { get; set; } // e.g. 500mg
        public string Frequency { get; set; } // e.g. Twice a day
        public string Duration { get; set; } // e.g. 5 days
        public long PrescriptionId { get; set; }
    }
}
