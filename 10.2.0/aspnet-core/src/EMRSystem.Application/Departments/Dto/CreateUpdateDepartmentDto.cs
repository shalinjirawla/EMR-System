﻿using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMRSystem.Departments.Dto
{
    public class CreateUpdateDepartmentDto : EntityDto<long>
    {
        public int TenantId { get; set; }
        public string Name { get; set; }
    }
}
