using Abp.Application.Services;
using EMRSystem.Admissions.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMRSystem.Admissions
{
    public interface IAdmissionAppService : IAsyncCrudAppService<
        AdmissionDto, long, PagedAdmissionResultRequestDto, CreateUpdateAdmissionDto, CreateUpdateAdmissionDto>
    { }
}
