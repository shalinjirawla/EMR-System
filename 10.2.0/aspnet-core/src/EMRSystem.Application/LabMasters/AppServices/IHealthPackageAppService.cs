using Abp.Application.Services;
using Abp.Application.Services.Dto;
using EMRSystem.LabMasters.Dto.HealthPackage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMRSystem.LabMasters.AppServices
{
    public interface IHealthPackageAppService :
        IAsyncCrudAppService< // CRUD interface
            HealthPackageDto,               // Get
            long,                            // Primary Key
            PagedHealthPackageResultRequestDto, // Paged request
            CreateUpdateHealthPackageDto,    // Create
            CreateUpdateHealthPackageDto     // Update
        >
    {
    }
}
