using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMRSystem.Medicines.Dto
{
    public class AllocateMedicineResultDto
    {
        public long MedicineId { get; set; }
        public string MedicineName { get; set; }
        public string BatchNo { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public DateTime? ExpiryDate { get; set; }
    }

}
