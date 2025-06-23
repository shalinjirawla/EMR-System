using Abp.Application.Services.Dto;
using Abp.Runtime.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMRSystem.Prescriptions.Dto
{
    public class PagedPrescriptionResultRequestDto : PagedResultRequestDto, IShouldNormalize
    {
        public string Keyword { get; set; }
        public string Sorting { get; set; }

        public void Normalize()
        {
            if (!string.IsNullOrEmpty(Sorting))
            {
                Sorting = Sorting.ToLowerInvariant();
                if (Sorting.Contains("patientname"))
                {
                    Sorting = Sorting.Replace("patientname", "Patient.FullName");
                }
                else if (Sorting.Contains("doctorName"))
                {
                    Sorting = Sorting.Replace("doctorName", "Doctor.FullName");
                }
                // Add more mappings as needed
            }
            else
            {
                Sorting = "Patient.FullName, Doctor.FullName";
            }

            Keyword = Keyword?.Trim();
        }

    }
}
