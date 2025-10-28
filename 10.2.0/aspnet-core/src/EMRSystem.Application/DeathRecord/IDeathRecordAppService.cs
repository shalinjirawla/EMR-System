using Abp.Application.Services;
using Abp.Application.Services.Dto;
using EMRSystem.DeathRecord.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMRSystem.DeathRecord
{
    public interface IDeathRecordAppService :
         IAsyncCrudAppService<
             DeathRecordDto,             // DTO for display
             long,                       // Primary key type
             PagedDeathRecordResultRequestDto, // For pagination/sorting
             CreateUpdateDeathRecordDto, // For create
             CreateUpdateDeathRecordDto  // For update
         >
    {
    }
}
