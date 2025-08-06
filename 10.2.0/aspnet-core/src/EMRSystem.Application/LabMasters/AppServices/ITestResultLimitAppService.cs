using Abp.Application.Services;
using Abp.Application.Services.Dto;
using EMRSystem.LabMasters.Dto.TestResultLimit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMRSystem.LabMasters.AppServices
{
    public interface ITestResultLimitAppService : IAsyncCrudAppService<TestResultLimitDto, long, PagedTestResultLimitResultRequestDto, CreateUpdateTestResultLimitDto, CreateUpdateTestResultLimitDto>
    {
    }
}
