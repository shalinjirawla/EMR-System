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
    public interface ISelectedEmergencyProceduresAppService :
        IAsyncCrudAppService<
            SelectedEmergencyProceduresDto, // DTO to return
            long,                           // Primary key
            PagedAndSortedResultRequestDto, // Paging input
            CreateUpdateSelectedEmergencyProceduresDto, // Create input
            CreateUpdateSelectedEmergencyProceduresDto  // Update input
        >
    {
        Task MarkAsCompleteAsync(long id);

    }
}
