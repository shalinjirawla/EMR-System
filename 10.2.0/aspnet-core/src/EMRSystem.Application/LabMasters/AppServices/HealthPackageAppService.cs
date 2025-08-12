using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Domain.Repositories;
using Abp.Extensions;
using Abp.Linq.Extensions;
using AutoMapper.Internal.Mappers;
using EMRSystem.LabMasters.Dto.HealthPackage;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMRSystem.LabMasters.AppServices
{
    public class HealthPackageAppService :AsyncCrudAppService<HealthPackage,HealthPackageDto,long,PagedHealthPackageResultRequestDto, 
            CreateUpdateHealthPackageDto,    
            CreateUpdateHealthPackageDto      
        >,
        IHealthPackageAppService
    {
        private readonly IRepository<HealthPackageLabReportsType, long> _healthPackageLabReportsTypeRepository;

        public HealthPackageAppService(
            IRepository<HealthPackage, long> repository,
            IRepository<HealthPackageLabReportsType, long> healthPackageLabReportsTypeRepository
        ) : base(repository)
        {
            _healthPackageLabReportsTypeRepository = healthPackageLabReportsTypeRepository;
        }

        protected override IQueryable<HealthPackage> CreateFilteredQuery(PagedHealthPackageResultRequestDto input)
        {
            return Repository
                .GetAll()
                .Include(x => x.PackageReportTypes)
                    .ThenInclude(pr => pr.LabReportsType)
                        .ThenInclude(lrt => lrt.ReportTypeItems)   // load items
                            .ThenInclude(rti => rti.LabTest)       // load LabTest for each item
                .WhereIf(!input.Keyword.IsNullOrWhiteSpace(),
                    x => x.PackageName.Contains(input.Keyword) ||
                         x.Description.Contains(input.Keyword))
                .WhereIf(input.IsActive.HasValue, x => x.IsActive == input.IsActive);
        }

        public override async Task<HealthPackageDto> GetAsync(EntityDto<long> input)
        {
            var package = await Repository
                .GetAll()
                .Include(x => x.PackageReportTypes)
                .FirstOrDefaultAsync(x => x.Id == input.Id);

            if (package == null)
            {
                throw new Abp.UI.UserFriendlyException("Health package not found.");
            }

            var dto = ObjectMapper.Map<HealthPackageDto>(package);

            // Sirf IDs agar chahiye to
            dto.LabReportsTypeIds = package.PackageReportTypes
                .Select(pr => pr.LabReportsTypeId)
                .ToList();

            // Agar details nahi chahiye to clean kar do
            dto.LabReportsTypes = null;

            return dto;
        }



        public override async Task<HealthPackageDto> CreateAsync(CreateUpdateHealthPackageDto input)
        {
            var healthPackage = ObjectMapper.Map<HealthPackage>(input);
            await Repository.InsertAsync(healthPackage);
            await CurrentUnitOfWork.SaveChangesAsync();

            // Save related LabReportsTypes
            if (input.LabReportsTypes != null && input.LabReportsTypes.Count > 0)
            {
                foreach (var item in input.LabReportsTypes)
                {
                    var rel = new HealthPackageLabReportsType
                    {
                        TenantId = input.TenantId,
                        HealthPackageId = healthPackage.Id,
                        LabReportsTypeId = item.LabReportsTypeId
                    };
                    await _healthPackageLabReportsTypeRepository.InsertAsync(rel);
                }
            }
            return MapToEntityDto(healthPackage);
        }

        public override async Task<HealthPackageDto> UpdateAsync(CreateUpdateHealthPackageDto input)
        {
            var healthPackage = await Repository
                .GetAll()
                .Include(x => x.PackageReportTypes)
                .FirstOrDefaultAsync(x => x.Id == input.Id);

            ObjectMapper.Map(input, healthPackage);
            await Repository.UpdateAsync(healthPackage);

            // Delete old relations
            var oldRelations = _healthPackageLabReportsTypeRepository.GetAll()
                .Where(x => x.HealthPackageId == healthPackage.Id)
                .ToList();

            foreach (var rel in oldRelations)
            {
                await _healthPackageLabReportsTypeRepository.DeleteAsync(rel);
            }

            // Insert new relations
            if (input.LabReportsTypes != null && input.LabReportsTypes.Count > 0)
            {
                foreach (var item in input.LabReportsTypes)
                {
                    var rel = new HealthPackageLabReportsType
                    {
                        TenantId = input.TenantId,
                        HealthPackageId = healthPackage.Id,
                        LabReportsTypeId = item.LabReportsTypeId
                    };
                    await _healthPackageLabReportsTypeRepository.InsertAsync(rel);
                }
            }

            return MapToEntityDto(healthPackage);
        }
    }
}
