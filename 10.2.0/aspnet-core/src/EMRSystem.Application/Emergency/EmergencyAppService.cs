using Abp.Application.Services;
using Abp.Domain.Repositories;
using Abp.Extensions;
using Abp.Linq.Extensions;
using EMRSystem.Appointments.Dto;
using EMRSystem.Appointments;
using EMRSystem.Emergency.EmergencyCase;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using Abp.Domain.Entities;

namespace EMRSystem.Emergency.EmergencyCase
{
    public class EmergencyAppService :
         AsyncCrudAppService<EmergencyCase, EmergencyCaseDto, long, PagedEmergencyCaseResultRequestDto, CreateUpdateEmergencyCaseDto, CreateUpdateEmergencyCaseDto>,
         IEmergencyAppService
    {
        public EmergencyAppService(IRepository<EmergencyCase, long> repository)
            : base(repository)
        {
        }

        protected override IQueryable<EmergencyCase> CreateFilteredQuery(PagedEmergencyCaseResultRequestDto input)
        {
            return Repository.GetAll()
                .Include(x => x.Patient)
                .Include(x => x.Doctor)
                .Include(x => x.Nurse)
                .WhereIf(!input.Keyword.IsNullOrWhiteSpace(),
                    x => x.Patient.FullName.Contains(input.Keyword));
        }
        public override async Task<EmergencyCaseDto> CreateAsync(CreateUpdateEmergencyCaseDto input)
        {
            var entity = ObjectMapper.Map<EmergencyCase>(input);

            entity.EmergencyNumber = $"ER-{DateTime.Now.Year}-{Guid.NewGuid().ToString("N").Substring(0, 4).ToUpper()}";

            await Repository.InsertAsync(entity);
            return MapToEntityDto(entity);
        }
        public async Task UpdateEmergencyCase(CreateUpdateEmergencyCaseDto input)
        {
            if (string.IsNullOrEmpty(input.EmergencyNumber))
            {
                input.EmergencyNumber = $"ER-{DateTime.Now.Year}-{Guid.NewGuid().ToString("N").Substring(0, 4).ToUpper()}";
            }
            var emergencyCase = ObjectMapper.Map<EmergencyCase>(input);
            await Repository.UpdateAsync(emergencyCase);
            CurrentUnitOfWork.SaveChanges();
        }
    }
}
