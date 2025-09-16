using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMRSystem.Medicines.Dto
{
    public class MedicineMasterDto : EntityDto<long>
    {
        public int TenantId { get; set; }
        public string Name { get; set; }
        public long MedicineFormId { get; set; }
        public string FormName { get; set; }
        public decimal? Strength { get; set; }
        public long? StrengthUnitId { get; set; }
        public string StrengthUnitName { get; set; }
        public int MinimumStock { get; set; }
        public string Description { get; set; }
        public bool IsAvailable { get; set; }
    }
}
