using Abp.Application.Services.Dto;
using Abp.Application.Services;
using EMRSystem.Prescriptions.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMRSystem.Prescriptions
{
    public interface IPrescriptionItemsAppService : IAsyncCrudAppService<
    PrescriptionItemDto, long, PagedAndSortedResultRequestDto, CreateUpdatePrescriptionItemDto, CreateUpdatePrescriptionItemDto>
    {
    }
}
