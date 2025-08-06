using Abp.Application.Services;
using Abp.Application.Services.Dto;
using EMRSystem.LabMasters.Dto.MeasureUnit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMRSystem.LabMasters.AppServices
{
    public interface IMeasureUnitAppService : IAsyncCrudAppService<
        MeasureUnitDto,
        long,
        PagedMeasureUnitResultRequestDto,
        CreateUpdateMeasureUnitDto,
        CreateUpdateMeasureUnitDto>
    {
    }
}
