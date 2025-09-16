using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Domain.Repositories;
using Abp.Linq.Extensions;
using EMRSystem.EmergencyProcedure.Dto;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMRSystem.EmergencyProcedure
{
    public class SelectedEmergencyProceduresAppService :
        AsyncCrudAppService<
            SelectedEmergencyProcedures,
            SelectedEmergencyProceduresDto,
            long,
            PagedAndSortedResultRequestDto,
            CreateUpdateSelectedEmergencyProceduresDto,
            CreateUpdateSelectedEmergencyProceduresDto>,
        ISelectedEmergencyProceduresAppService
    {
        public SelectedEmergencyProceduresAppService(
            IRepository<SelectedEmergencyProcedures, long> repository
        ) : base(repository)
        {
        }
        protected override IQueryable<SelectedEmergencyProcedures> CreateFilteredQuery(PagedAndSortedResultRequestDto input)
        {
            return Repository.GetAll()
                .Include(x => x.EmergencyProcedures)
                .Include(x => x.Prescriptions)
                    .ThenInclude(p => p.Patient)
                .Where(x => x.IsPaid == true);
        }
        public async Task MarkAsCompleteAsync(long id)
        {
            var procedure = await Repository.GetAsync(id);
            procedure.Status = EmergencyProcedureStatus.Completed;
            await Repository.UpdateAsync(procedure);
        }

        public async Task<List<SelectedEmergencyProceduresDto>> GetSelectedProceduresByPatientID(long patientID)
        {
            var list = await Repository.GetAll()
                .Include(x => x.EmergencyProcedures)
                .Include(x => x.Prescriptions)
                    .ThenInclude(p => p.Patient)
                .Where(x => x.Prescriptions.PatientId == patientID).ToListAsync();

            var mappedRes = ObjectMapper.Map<List<SelectedEmergencyProceduresDto>>(list);
            return mappedRes;
        }
    }
}
