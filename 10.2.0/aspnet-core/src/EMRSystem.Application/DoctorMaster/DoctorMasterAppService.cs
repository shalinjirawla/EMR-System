using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Domain.Repositories;
using Abp.Linq.Extensions;
using AutoMapper.Internal.Mappers;
using EMRSystem.DoctorMaster.Dto;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMRSystem.DoctorMaster
{
    public class DoctorMasterAppService : AsyncCrudAppService<
       DoctorMaster, DoctorMasterDto, long, PagedDoctorMasterResultRequestDto,
       CreateUpdateDoctorMasterDto, CreateUpdateDoctorMasterDto>,
       IDoctorMasterAppService
    {
        public DoctorMasterAppService(IRepository<DoctorMaster, long> repository)
            : base(repository)
        {
        }

        protected override IQueryable<DoctorMaster> CreateFilteredQuery(PagedDoctorMasterResultRequestDto input)
        {
            return Repository.GetAll()
                .Include(x => x.Doctor)
                .WhereIf(
                    !string.IsNullOrWhiteSpace(input.Keyword),
                    x => EF.Functions.Like(x.Doctor.FullName, $"%{input.Keyword}%")
                );
        }

        public async Task<ListResultDto<DoctorMasterDto>> GetAllByTenantIdAsync(int tenantId)
        {
            var all = await Repository
                .GetAllIncluding(x => x.Doctor)
                .Where(x => x.TenantId == tenantId)
                .ToListAsync();

            return new ListResultDto<DoctorMasterDto>(ObjectMapper.Map<List<DoctorMasterDto>>(all));
        }
    }
}
