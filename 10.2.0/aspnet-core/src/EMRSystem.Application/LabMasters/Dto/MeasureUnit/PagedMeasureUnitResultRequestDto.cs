using Abp.Application.Services.Dto;
using Abp.Runtime.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMRSystem.LabMasters.Dto.MeasureUnit
{
    public class PagedMeasureUnitResultRequestDto : PagedAndSortedResultRequestDto, IShouldNormalize
    {
        public string Keyword { get; set; }
        public bool? IsActive { get; set; }
        public string Sorting { get; set; }

        public void Normalize()
        {
            Keyword = Keyword?.Trim();

            if (string.IsNullOrWhiteSpace(Sorting))
            {
                Sorting = "Id desc"; // ✅ must include direction
            }
            else
            {
                Sorting = Sorting.Trim();
                if (Sorting.StartsWith("name", StringComparison.OrdinalIgnoreCase))
                    Sorting = Sorting.Replace("name", "Name", StringComparison.OrdinalIgnoreCase);
            }
        }
    }
}
