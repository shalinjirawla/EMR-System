using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Domain.Repositories;
using Abp.Runtime.Session;
using AutoMapper.Internal.Mappers;
using EMRSystem.AppointmentType.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMRSystem.AppointmentType
{
    public class AppointmentTypeAppService : AsyncCrudAppService<AppointmentType, AppointmentTypeDto, long, PagedResultRequestDto, CreateUpdateAppointmentTypeDto, CreateUpdateAppointmentTypeDto>, IAppointmentTypeAppService
    {
        public AppointmentTypeAppService(IRepository<AppointmentType, long> repository)
            : base(repository)
        {
        }

        protected override IQueryable<AppointmentType> CreateFilteredQuery(PagedResultRequestDto input)
        {
            return base.CreateFilteredQuery(input)
                .Where(x => x.TenantId == AbpSession.TenantId);
        }

        public async Task<ListResultDto<AppointmentTypeDto>> GetAllForTenant()
        {
            var types = await Repository.GetAllListAsync(x => x.TenantId == AbpSession.TenantId);
            return new ListResultDto<AppointmentTypeDto>(ObjectMapper.Map<List<AppointmentTypeDto>>(types));
        }
    }
}
