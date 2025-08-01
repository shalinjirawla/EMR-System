﻿using Abp.Application.Services.Dto;
using Abp.Runtime.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMRSystem.Patients.Dto
{
    public class PagedPatientResultRequestDto : PagedResultRequestDto, IShouldNormalize
    {
        public string Keyword { get; set; }

        public string Sorting { get; set; }


        public void Normalize()
        {
            if (!string.IsNullOrEmpty(Sorting))
            {
                Sorting = Sorting.ToLowerInvariant();
                if (Sorting.Contains("fullname"))
                {
                    Sorting = Sorting.Replace("fullname", "FullName");
                }
                else if (Sorting.Contains("emailaddress"))
                {
                    // Change to sort by navigation property
                    Sorting = Sorting.Replace("emailaddress", "AbpUser.EmailAddress");
                }
            }
            else
            {
                Sorting = "FullName";
            }
            Keyword = Keyword?.Trim();
        }
    }
}
