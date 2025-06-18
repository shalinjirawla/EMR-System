using Abp.Application.Services.Dto;
using Abp.Application.Services;
using Abp.Domain.Repositories;
using EMRSystem.Prescriptions.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMRSystem.Prescriptions
{
    public class PrescriptionItemsAppService : AsyncCrudAppService<PrescriptionItem, PrescriptionItemDto, long, PagedAndSortedResultRequestDto, CreateUpdatePrescriptionItemDto, CreateUpdatePrescriptionItemDto>,
   IPrescriptionItemsAppService
    {
        public PrescriptionItemsAppService(IRepository<PrescriptionItem, long> repository) : base(repository)
        {
        }
    }
}
