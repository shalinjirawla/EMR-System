using Abp.Application.Services.Dto;
using Abp.Application.Services;
using EMRSystem.Nurse.Dto;
using EMRSystem.Nurse;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EMRSystem.Patient_Discharge.Dto;
using Abp.Domain.Repositories;
using Abp.Domain.Entities;
using Abp.UI;
using Microsoft.AspNetCore.Mvc;
using EMRSystem.Authorization.Users;
using EMRSystem.Users.Dto;
using Abp.Collections.Extensions;
using Microsoft.EntityFrameworkCore;
using EMRSystem.PatientDischarge;

namespace EMRSystem.Patient_Discharge
{
    public class PatientDischargeAppService : AsyncCrudAppService<EMRSystem.PatientDischarge.PatientDischarge, PatientDischargeDto, long, PagedAndSortedResultRequestDto, CreateUpdatePatientDischargeDto, CreateUpdatePatientDischargeDto>,
   IPatientDischargeAppService
    {
        public PatientDischargeAppService(IRepository<EMRSystem.PatientDischarge.PatientDischarge, long> repository
            ) : base(repository)
        {
        }

        protected override IQueryable<EMRSystem.PatientDischarge.PatientDischarge> CreateFilteredQuery(PagedAndSortedResultRequestDto input)
        {
            var data = Repository.GetAllIncluding(x => x.Admission, x => x.Admission.Room, x => x.Admission.Room.RoomTypeMaster, x => x.Admission.Bed, x => x.Patient, x => x.Doctor);
            return data;
        }
        public async Task InitiateDischarge(long dischargeId)
        {
            try
            {
                var discharge = await Repository.GetAllIncluding(x => x.Patient).FirstOrDefaultAsync(x => x.PatientId == dischargeId);

                if (discharge?.DischargeStatus != DischargeStatus.Pending)
                    throw new UserFriendlyException("Discharge already initiated or processed.");

                discharge.DischargeStatus = DischargeStatus.Initiated;
                await Repository.UpdateAsync(discharge);
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException("No Patient Data found");
            }
        }
    }
}
