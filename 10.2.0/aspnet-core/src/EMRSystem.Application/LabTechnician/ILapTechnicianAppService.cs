using Abp.Application.Services.Dto;
using Abp.Application.Services;
using EMRSystem.LabTechnician.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMRSystem.LabTechnician
{
    public interface ILapTechnicianAppService :
        IAsyncCrudAppService<
              LabTechniciansDto,
            long,
            PagedAndSortedResultRequestDto,
            CreateUpdateLabTechnicianDto,
            CreateUpdateLabTechnicianDto>
    {
    }
}
