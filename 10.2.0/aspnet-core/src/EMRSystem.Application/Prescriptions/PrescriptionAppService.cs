using Abp.Application.Services.Dto;
using Abp.Application.Services;
using Abp.Domain.Repositories;
using EMRSystem.Prescriptions.Dto;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq.Dynamic.Core;

namespace EMRSystem.Prescriptions
{
    public class PrescriptionAppService : AsyncCrudAppService<Prescription, PrescriptionDto, long, PagedAndSortedResultRequestDto, CreateUpdatePrescriptionDto, CreateUpdatePrescriptionDto>,
   IPrescriptionAppService
    {
        public PrescriptionAppService(IRepository<Prescription, long> repository
            ) : base(repository)
        {
        }
    }
}
