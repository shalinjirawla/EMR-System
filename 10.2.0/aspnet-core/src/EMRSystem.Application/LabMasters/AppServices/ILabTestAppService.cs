using Abp.Application.Services;
using Abp.Application.Services.Dto;
using EMRSystem.LabMasters.Dto.LabTest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMRSystem.LabMasters.AppServices
{
    public interface ILabTestAppService : IAsyncCrudAppService<
        LabTestDto,
        long,
        PagedLabTestResultRequestDto,
        CreateUpdateLabTestDto,
        CreateUpdateLabTestDto>
    {
    }
}
