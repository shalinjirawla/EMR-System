using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Domain.Repositories;
using EMRSystem.Prescriptions;
using EMRSystem.Prescriptions.Dto;
using EMRSystem.Vitals.Dto;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMRSystem.Vitals
{
    public class VitalAppService : AsyncCrudAppService<Vital, VitalDto, long, PagedAndSortedResultRequestDto, CreateUpdateVitalDto, CreateUpdateVitalDto>,
  IVitalAppService
    {
        public VitalAppService(IRepository<Vital, long> repository) : base(repository)
        {
        }

        public override async Task<VitalDto> CreateAsync(CreateUpdateVitalDto input)
        {
            try
            {
                return await base.CreateAsync(input);
            }
            catch (Exception ex)
            {
                //Logger.LogError(ex, "Error in CreateAsync");
                throw;
            }
        }
        protected override IQueryable<Vital> CreateFilteredQuery(PagedAndSortedResultRequestDto input)
        {
            return Repository
                .GetAll()
                .Include(x => x.Patient)
                .Include(x => x.Nurse);
        }
        protected override async Task<Vital> GetEntityByIdAsync(long id)
        {
            return await Repository
                .GetAll()
                .Include(x => x.Patient)
                .Include(x => x.Nurse)
                .FirstOrDefaultAsync(x => x.Id == id);
        }



    }
}
