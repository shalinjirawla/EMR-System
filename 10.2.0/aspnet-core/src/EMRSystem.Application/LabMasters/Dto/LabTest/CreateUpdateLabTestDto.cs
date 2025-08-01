﻿using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMRSystem.LabMasters.Dto.LabTest
{
    public class CreateUpdateLabTestDto : EntityDto<long>
    {
        public int TenantId { get; set; }
        public string Name { get; set; }
        public long MeasureUnitId { get; set; }
        public bool IsActive { get; set; }
    }
}
