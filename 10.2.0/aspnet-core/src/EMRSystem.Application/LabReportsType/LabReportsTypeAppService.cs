using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.UI;
using EMRSystem.Authorization;
using EMRSystem.Doctor.Dto;
using EMRSystem.LabMasters;
using EMRSystem.LabReportsType;
using EMRSystem.LabReportsType.Dto;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMRSystem.LabReportsTypes
{
    //[AbpAuthorize(PermissionNames.Pages_Users)]
    public class LabReportsTypeAppService : AsyncCrudAppService<LabReportsType, LabReportsTypeDto, long, PagedAndSortedResultRequestDto, CreateUpdateLabReportTypeDto, CreateUpdateLabReportTypeDto>,
  ILabReportsTypeAppService
    {
        private readonly IRepository<LabReportsType, long> _labReportTypeRepository;
        private readonly IRepository<LabReportTypeItem, long> _labReportTypeItemRepository;
        private readonly IRepository<LabTest, long> _labTestRepository;
        private readonly IRepository<HealthPackage, long> _healthPackageRepository;
        private readonly IRepository<TestResultLimit, long> _testResultLimitRepository;
        public LabReportsTypeAppService(IRepository<LabReportsType, long> repository,
            IRepository<LabReportsType, long> labReportTypeRepository,
            IRepository<LabReportTypeItem, long> labReportTypeItemRepository,
            IRepository<LabTest, long> labTestRepository,
            IRepository<HealthPackage, long> healthPackageRepository,
            IRepository<TestResultLimit, long> testResultLimitRepository) : base(repository)
        {
            _labReportTypeRepository = labReportTypeRepository;
            _labReportTypeItemRepository = labReportTypeItemRepository;
            _labTestRepository = labTestRepository;
            _testResultLimitRepository = testResultLimitRepository;
            _healthPackageRepository = healthPackageRepository;
        }

        public async Task<ListResultDto<LabReportsTypeDto>> GetAllTestByTenantID(int tenantId)
        {
            var query = Repository.GetAll()
                .Where(x => x.TenantId == tenantId); 
            var labReports = await query.ToListAsync();
            var mapped = ObjectMapper.Map<List<LabReportsTypeDto>>(labReports);
            return new ListResultDto<LabReportsTypeDto>(mapped);
        }
        [HttpGet]
        public async Task<ListResultDto<LabTestOrPackageDto>> GetAllTestsAndPackagesByTenantId(int tenantId)
        {
            // Get all single lab tests
            var labTests = await Repository.GetAll()
                .Where(x => x.TenantId == tenantId)
                .Select(x => new LabTestOrPackageDto
                {
                    Id = x.Id,
                    Name = x.ReportType,
                    Price = x.ReportPrice,
                    Type = "Test",
                    PackageTestIds = null // Not applicable
                })
                .ToListAsync();

            // Get all packages with their test IDs
            var packages = await _healthPackageRepository.GetAll()
                .Where(x => x.TenantId == tenantId && x.IsActive)
                .Select(x => new LabTestOrPackageDto
                {
                    Id = x.Id,
                    Name = x.PackageName,
                    Price = x.PackagePrice,
                    Type = "Package",
                    PackageTestIds = x.PackageReportTypes
                        .Select(p => p.LabReportsTypeId)
                        .ToList()
                })
                .ToListAsync();

            // Merge both
            var result = labTests.Concat(packages)
                .OrderBy(x => x.Name)
                .ToList();

            return new ListResultDto<LabTestOrPackageDto>(result);
        }



        public async Task<LabReportDetailDto> GetReportDetailsWithTestsAsync(long labReportTypeId)
        {
            var report = await _labReportTypeRepository.GetAsync(labReportTypeId);
            if (report == null)
            {
                throw new UserFriendlyException("Lab Report Type not found");
            }

            var reportItems = await _labReportTypeItemRepository
                .GetAll()
                .Where(x => x.LabReportTypeId == labReportTypeId)
                .ToListAsync();

            var labTestIds = reportItems.Select(x => x.LabTestId).Distinct().ToList();

            var labTests = await _labTestRepository
                .GetAllIncluding(x => x.MeasureUnit)
                .Where(x => labTestIds.Contains(x.Id))
                .ToListAsync();

            var resultLimits = await _testResultLimitRepository
                .GetAll()
                .Where(x => labTestIds.Contains(x.LabTestId))
                .ToListAsync();

            var labTestItems = labTests.Select(test =>
            {
                var limit = resultLimits.FirstOrDefault(r => r.LabTestId == test.Id);
                return new LabTestItemDto
                {
                    LabTestId = test.Id,
                    LabTestName = test.Name,
                    MeasureUnitName = test.MeasureUnit?.Name,
                    MinRange = limit?.MinRange,
                    MaxRange = limit?.MaxRange
                };
            }).ToList();

            return new LabReportDetailDto
            {
                ReportName = report.ReportType,
                ReportPrice = report.ReportPrice,
                LabTests = labTestItems
            };
        }

        //public async Task<List<PackageSuggestionDto>> GetPackageSuggestionsAsync(List<long> selectedLabReportTypeIds)
        //{
        //    var suggestions = new List<PackageSuggestionDto>();

        //    // 1️⃣ Get all packages including their linked tests
        //    var allPackages = await _healthPackageRepository
        //        .GetAllIncluding(x => x.PackageReportTypes)
        //        .ToListAsync();

        //    foreach (var package in allPackages)
        //    {
        //        var packageTestIds = package.PackageReportTypes
        //            .Select(p => p.LabReportsTypeId)
        //            .ToList();

        //        // 2️⃣ Find how many selected tests this package covers
        //        var matchedTests = selectedLabReportTypeIds
        //            .Where(id => packageTestIds.Contains(id))
        //            .ToList();

        //        // Skip if package covers none of the selected tests
        //        if (!matchedTests.Any())
        //            continue;

        //        // 3️⃣ Calculate selected tests total price
        //        var queryableTests = Repository.GetAll();

        //        decimal selectedTestsTotalPrice = await queryableTests
        //            .Where(t => matchedTests.Contains(t.Id))
        //            .SumAsync(t => t.ReportPrice);

        //        decimal packagePrice = package.PackagePrice;

        //        // 4️⃣ Add suggestion
        //        suggestions.Add(new PackageSuggestionDto
        //        {
        //            PackageId = package.Id,
        //            PackageName = package.PackageName,
        //            IncludedTests = packageTestIds,
        //            SelectedTestsPrice = selectedTestsTotalPrice,
        //            PackagePrice = packagePrice,
        //            MatchCount = matchedTests.Count,
        //            TotalSelectedCount = selectedLabReportTypeIds.Count,
        //            ExtraTests = packageTestIds
        //                .Where(tid => !selectedLabReportTypeIds.Contains(tid))
        //                .ToList()
        //        });
        //    }

        //    // 5️⃣ Sort suggestions: highest match first, then cheapest
        //    return suggestions
        //        .OrderByDescending(s => s.MatchCount)
        //        .ThenBy(s => s.PackagePrice)
        //        .ToList();
        //}

        public async Task<List<PackageSuggestionDto>> GetPackageSuggestionsAsync(List<long> selectedLabReportTypeIds)
        {
            var suggestions = new List<PackageSuggestionDto>();

            // 1️⃣ Get all packages including their linked tests
            var allPackages = await _healthPackageRepository
                .GetAllIncluding(x => x.PackageReportTypes)
                .ToListAsync();

            foreach (var package in allPackages)
            {
                var packageTestIds = package.PackageReportTypes
                    .Select(p => p.LabReportsTypeId)
                    .ToList();

                // ✅ Only consider packages where ALL its tests are in the selected list
                bool allTestsMatch = packageTestIds.All(id => selectedLabReportTypeIds.Contains(id));

                if (!allTestsMatch)
                {
                    // ✅ Suggest if selected tests are a subset of package tests (partial match)
                    bool partialMatch = selectedLabReportTypeIds.All(id => packageTestIds.Contains(id));

                    if (!partialMatch) continue;
                }

                // 3️⃣ Calculate total selected tests price (only those in the package)
                var queryableTests = Repository.GetAll();
                decimal selectedTestsTotalPrice = await queryableTests
                    .Where(t => packageTestIds.Contains(t.Id))
                    .SumAsync(t => t.ReportPrice);

                decimal packagePrice = package.PackagePrice;

                // 4️⃣ Add suggestion
                suggestions.Add(new PackageSuggestionDto
                {
                    PackageId = package.Id,
                    PackageName = package.PackageName,
                    IncludedTests = packageTestIds,
                    SelectedTestsPrice = selectedTestsTotalPrice,
                    PackagePrice = packagePrice,
                    MatchCount = packageTestIds.Count,
                    TotalSelectedCount = selectedLabReportTypeIds.Count,
                    ExtraTests = packageTestIds.Except(selectedLabReportTypeIds).ToList() // ✅ No extra tests since it's a full match
                });
            }

            // 5️⃣ Sort suggestions: highest match first, then cheapest
            return suggestions
                .OrderByDescending(s => s.MatchCount)
                .ThenBy(s => s.PackagePrice)
                .ToList();
        }




    }
}
