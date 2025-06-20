using Abp.Application.Services.Dto;
using Abp.Application.Services;
using Abp.Authorization;
using Abp.Domain.Repositories;
using EMRSystem.Authorization;
using EMRSystem.LabReports;
using EMRSystem.LabTechnician.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMRSystem.LabTechnician
{
    [AbpAuthorize(PermissionNames.Pages_LabReports)]
    public class LapTechnicianAppService :
        AsyncCrudAppService<EMRSystem.LabReports.LabTechnician,LabTechniciansDto,long,PagedAndSortedResultRequestDto,CreateUpdateLabTechnicianDto,CreateUpdateLabTechnicianDto>,
            ILapTechnicianAppService
    {
        public LapTechnicianAppService(IRepository<EMRSystem.LabReports.LabTechnician, long> repository) : base(repository)
        {
        }
    }
}
