using Abp.Application.Services.Dto;
using Abp.Application.Services;
using EMRSystem.Doctor.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMRSystem.Doctor
{
    public interface IConsultationRequestsAppService : IAsyncCrudAppService<
ConsultationRequestsDto, long, PagedAndSortedResultRequestDto, CreateUpdateConsultationRequestsDto, CreateUpdateConsultationRequestsDto>
    {
    }
}
