using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMRSystem.Medicines.Dto
{
    public class MedicineWithStockDto
    {
        public long Id { get; set; }
        public string MedicineName { get; set; }
        public string FormName { get; set; }
        public decimal? Strength { get; set; }
        public string StrengthUnitName { get; set; }
        public List<MedicineStockDto> Stocks { get; set; }
    }
}
