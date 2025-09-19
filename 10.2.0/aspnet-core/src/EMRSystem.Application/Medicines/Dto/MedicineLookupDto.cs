using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMRSystem.Medicines.Dto
{
    public class MedicineLookupDto
    {
        public long Id { get; set; }
        public string MedicineName { get; set; }
        public long MedicineFormId { get; set; }
        public string DosageOption { get; set; } // e.g., "500 mg"
    }

}
