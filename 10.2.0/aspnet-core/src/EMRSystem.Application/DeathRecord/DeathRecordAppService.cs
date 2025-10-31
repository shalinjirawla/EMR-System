using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Domain.Repositories;
using Abp.Linq.Extensions;
using AutoMapper.Internal.Mappers;
using EMRSystem.DeathRecord.Dto;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMRSystem.DeathRecord
{
    public class DeathRecordAppService :
        AsyncCrudAppService<
            DeathRecord,                  
            DeathRecordDto,               
            long,
            PagedDeathRecordResultRequestDto, 
            CreateUpdateDeathRecordDto,     
            CreateUpdateDeathRecordDto    
        >,
        IDeathRecordAppService
    {
        public DeathRecordAppService(IRepository<DeathRecord, long> repository)
            : base(repository)
        {
        }

        protected override IQueryable<DeathRecord> CreateFilteredQuery(PagedDeathRecordResultRequestDto input)
        {
            return Repository.GetAllIncluding(x => x.Patient, x => x.Doctor, x => x.Nurse)
                .WhereIf(!string.IsNullOrWhiteSpace(input.Keyword),
                        x => x.Patient.FullName.ToLower().Contains(input.Keyword.ToLower()) ||
                        x.Doctor.FullName.ToLower().Contains(input.Keyword.ToLower()) ||
                        x.Nurse.FullName.ToLower().Contains(input.Keyword.ToLower()));
        }

        public override async Task<DeathRecordDto> GetAsync(EntityDto<long> input)
        {
            var entity = await Repository.GetAllIncluding(x => x.Patient, x => x.Doctor, x => x.Nurse)
                .FirstOrDefaultAsync(x => x.Id == input.Id);

            return ObjectMapper.Map<DeathRecordDto>(entity);
        }
    }
}
