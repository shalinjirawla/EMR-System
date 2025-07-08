using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Domain.Repositories;
using Abp.Extensions;
using Abp.Linq.Extensions;
using EMRSystem.Appointments;
using EMRSystem.Appointments.Dto;
using EMRSystem.Authorization.Users;
using EMRSystem.Prescriptions;
using EMRSystem.Prescriptions.Dto;
using EMRSystem.Users.Dto;
using EMRSystem.Vitals.Dto;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Text;
using System.Threading.Tasks;


namespace EMRSystem.Vitals
{
    public class VitalAppService : AsyncCrudAppService<Vital, VitalDto, long, PagedVitalResultRequestDto, CreateUpdateVitalDto, CreateUpdateVitalDto>,
  IVitalAppService
    {
        private readonly UserManager _userManager;
        public VitalAppService(IRepository<Vital, long> repository, UserManager userManager) : base(repository)
        {
            _userManager = userManager;
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
        protected override IQueryable<Vital> CreateFilteredQuery(PagedVitalResultRequestDto input)
        {
            return Repository
                .GetAll()
                .Include(x => x.Patient)
                .Include(x => x.Nurse)
                .WhereIf(!input.Keyword.IsNullOrWhiteSpace(), x =>
                    x.Patient.FullName.Contains(input.Keyword) ||
                    x.Nurse.FullName.Contains(input.Keyword));
        }
        protected override IQueryable<Vital> ApplySorting(IQueryable<Vital> query, PagedVitalResultRequestDto input)
        {
            if (!string.IsNullOrWhiteSpace(input.Sorting))
            {
                var sorting = input.Sorting;

                if (sorting.Contains("patientName", StringComparison.OrdinalIgnoreCase))
                    sorting = sorting.Replace("patientName", "Patient.FullName", StringComparison.OrdinalIgnoreCase);

                if (sorting.Contains("nurseName", StringComparison.OrdinalIgnoreCase))
                    sorting = sorting.Replace("nurseName", "Nurse.FullName", StringComparison.OrdinalIgnoreCase);

                return query.OrderBy(sorting);
            }

            return base.ApplySorting(query, input);
        }

        protected override async Task<Vital> GetEntityByIdAsync(long id)
        {
            return await Repository
                .GetAll()
                .Include(x => x.Patient)
                .Include(x => x.Nurse)
                .FirstOrDefaultAsync(x => x.Id == id);
        }
        [HttpGet]
        public async Task<List<string>> GetCurrentUserRolesAsync()
        {
            if (!AbpSession.UserId.HasValue)
                return null;
            var user = await _userManager.GetUserByIdAsync(AbpSession.UserId.Value);
            var roles = await _userManager.GetRolesAsync(user);
            return roles.ToList();
        }
    }
}
