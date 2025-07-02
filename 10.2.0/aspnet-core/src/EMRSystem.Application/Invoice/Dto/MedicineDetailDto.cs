using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMRSystem.Invoice.Dto
{
    public class MedicineDetailDto
    {
        public long PrescriptionItemId { get; set; }
        public string MedicineName { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
    }
}
