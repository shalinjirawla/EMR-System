using Abp.Application.Services.Dto;
using Abp.Application.Services;
using Abp.Domain.Repositories;
using EMRSystem.Prescriptions.Dto;
using EMRSystem.Prescriptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EMRSystem.Vitals.Dto;

namespace EMRSystem.Vitals
{
    public class VitalAppService : AsyncCrudAppService<Vital, VitalDto, long, PagedAndSortedResultRequestDto, CreateUpdateVitalDto, CreateUpdateVitalDto>,
  IVitalAppService
    {
        public VitalAppService(IRepository<Vital, long> repository) : base(repository)
        {
        }
    }
}
