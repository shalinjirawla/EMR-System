using Abp.Application.Services;
using Abp.Application.Services.Dto;
using EMRSystem.EmergencyProcedure.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMRSystem.EmergencyProcedure
{
    public interface IEmergencyProcedureAppService :
        IAsyncCrudAppService<
            EmergencyProcedureDto,          // DTO to return
            long,                           // Primary key
            PagedEmergencyProcedureResultRequestDto, // For paging/sorting
            CreateUpdateEmergencyProcedureDto, // For create
            CreateUpdateEmergencyProcedureDto  // For update
        >
    {
    }
}
