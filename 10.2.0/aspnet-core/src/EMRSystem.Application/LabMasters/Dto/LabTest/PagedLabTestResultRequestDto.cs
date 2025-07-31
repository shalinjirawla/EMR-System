using Abp.Application.Services.Dto;
using Abp.Runtime.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMRSystem.LabMasters.Dto.LabTest
{
    public class PagedLabTestResultRequestDto : PagedResultRequestDto, IShouldNormalize
    {
        public string Keyword { get; set; }
        public bool? IsActive { get; set; }
        public string Sorting { get; set; }

        public void Normalize()
        {
            Keyword = Keyword?.Trim();

            if (!string.IsNullOrEmpty(Sorting))
            {
                Sorting = Sorting.ToLowerInvariant();
                if (Sorting.Contains("name"))
                {
                    Sorting = Sorting.Replace("name", "Name");
                }
            }
            else
            {
                Sorting = "Name"; // default
            }
        }
    }
}
