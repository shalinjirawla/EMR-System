﻿using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMRSystem.Admissions.Dto
{
    public class PagedAdmissionResultRequestDto : PagedResultRequestDto
    {
        public string Keyword { get; set; }
    }
}
