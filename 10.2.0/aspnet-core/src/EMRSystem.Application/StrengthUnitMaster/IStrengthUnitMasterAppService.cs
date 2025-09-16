using Abp.Application.Services;
using Abp.Application.Services.Dto;
using EMRSystem.StrengthUnitMaster.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMRSystem.StrengthUnitMaster
{
    public interface IStrengthUnitMasterAppService :
        IAsyncCrudAppService<StrengthUnitMasterDto, long, PagedAndSortedResultRequestDto, CreateUpdateStrengthUnitMasterDto, CreateUpdateStrengthUnitMasterDto>
    {
    }
}
