using Abp.Application.Services.Dto;
using Abp.Runtime.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMRSystem.Pharmacist.Dto
{
    public class PagedPharmacistInventoryResultRequestDto : PagedResultRequestDto, IShouldNormalize
    {
        public string Keyword { get; set; }
        public string Sorting { get; set; }
        public int? MinStock { get; set; }
        public int? MaxStock { get; set; }
        public DateTime? FromExpiryDate { get; set; }
        public DateTime? ToExpiryDate { get; set; }
        public bool? IsAvailable { get; set; }

        public void Normalize()
        {
            if (string.IsNullOrEmpty(Sorting))
            {
                Sorting = "MedicineName";
            }

            Keyword = Keyword?.Trim();
        }
    }
}
