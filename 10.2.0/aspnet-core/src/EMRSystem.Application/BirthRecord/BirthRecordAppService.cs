using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Domain.Repositories;
using Abp.Linq.Extensions;
using EMRSystem.BirthRecord.Dto;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMRSystem.BirthRecord
{
    public class BirthRecordAppService :
         AsyncCrudAppService<BirthRecord,BirthRecordDto, long, PagedBirthRecordResultRequestDto, CreateUpdateBirthRecordDto>,
         IBirthRecordAppService
    {
        public BirthRecordAppService(IRepository<BirthRecord, long> repository)
            : base(repository)
        {
        }

        protected override IQueryable<BirthRecord> CreateFilteredQuery(PagedBirthRecordResultRequestDto input)
        {
            return Repository.GetAll()
                   .Include(x => x.Mother)
                   .Include(x => x.Doctor)
                   .Include(x => x.Nurse)
                   .WhereIf(!string.IsNullOrWhiteSpace(input.Keyword),
                        x => x.Mother.FullName.ToLower().Contains(input.Keyword.ToLower()) ||
                        x.Doctor.FullName.ToLower().Contains(input.Keyword.ToLower()));
        }

        public override async Task<BirthRecordDto>GetAsync(EntityDto<long> input)
        {
            var entity = await Repository.GetAllIncluding(x => x.Mother, x => x.Doctor, x => x.Nurse)
                .FirstOrDefaultAsync(x => x.Id == input.Id);

            return ObjectMapper.Map<BirthRecordDto>(entity);

        }
    }
}
