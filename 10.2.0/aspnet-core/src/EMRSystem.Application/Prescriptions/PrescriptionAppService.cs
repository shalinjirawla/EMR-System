using Abp.Application.Services.Dto;
using Abp.Application.Services;
using Abp.Domain.Repositories;
using EMRSystem.Billings;
using EMRSystem.BillingStaff.Dto;
using EMRSystem.BillingStaff;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EMRSystem.Prescriptions.Dto;

namespace EMRSystem.Prescriptions
{
    public class PrescriptionAppService : AsyncCrudAppService<Prescription, PrescriptionDto, long, PagedAndSortedResultRequestDto, CreateUpdatePrescriptionDto, CreateUpdatePrescriptionDto>,
   IPrescriptionAppService
    {
        public PrescriptionAppService(IRepository<Prescription, long> repository) : base(repository)
        {
        }
    }
}
