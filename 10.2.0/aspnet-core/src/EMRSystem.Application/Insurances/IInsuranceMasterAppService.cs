using Abp.Application.Services;
using Abp.Application.Services.Dto;
using EMRSystem.Insurances.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMRSystem.Insurances
{
    public interface IInsuranceMasterAppService :
        IAsyncCrudAppService<InsuranceMasterDto, long, PagedInsuranceMasterDto,
            CreateUpdateInsuranceMasterDto, CreateUpdateInsuranceMasterDto>
    {
    }
}
