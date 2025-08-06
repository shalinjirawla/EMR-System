using Abp.Application.Services;
using Abp.Domain.Repositories;
using EMRSystem.Appointments.Dto;
using EMRSystem.Appointments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EMRSystem.LabReport.Dto;
using Abp.EntityFrameworkCore.Repositories;
using EMRSystem.LabReports;
using Abp.Application.Services.Dto;
using EMRSystem.LabTechnician;
using EMRSystem.LabTechnician.Dto;
using Microsoft.AspNetCore.Mvc;
using EMRSystem.MultiTenancy;
using Microsoft.EntityFrameworkCore;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace EMRSystem.LabReport
{
    public class LabReportResultItemAppService : AsyncCrudAppService<EMRSystem.LabReports.LabReportResultItem, LabReportResultItemDto, long, PagedAndSortedResultRequestDto, CreateUpdateLabReportResultItemDto, CreateUpdateLabReportResultItemDto>,
  ILabReportResultItemAppService
    {
        private readonly IPrescriptionLabTestAppService _prescriptionLabTestsAppService;
        private readonly TenantManager _tenantManager;
        public LabReportResultItemAppService(IRepository<EMRSystem.LabReports.LabReportResultItem, long> repository
            , IPrescriptionLabTestAppService prescriptionLabTestsAppService,
            TenantManager tenantManager
            ) : base(repository)
        {
            _prescriptionLabTestsAppService = prescriptionLabTestsAppService;
            _tenantManager = tenantManager;
        }
        public async Task AddLabReportResultItem(List<CreateUpdateLabReportResultItemDto> reportResultItemDtos)
        {
            try
            {
                if (reportResultItemDtos.Count > 0)
                {
                    var mapped = ObjectMapper.Map<List<LabReportResultItem>>(reportResultItemDtos);
                    await Repository.InsertRangeAsync(mapped);
                    CurrentUnitOfWork.SaveChanges();
                    var id = reportResultItemDtos[0].PrescriptionLabTestId;
                   // await _prescriptionLabTestsAppService.MakeInprogressReports(id);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        [HttpGet]
        public async Task<List<CreateUpdateLabReportResultItemDto>> GetLabReportResultItemsById(long prescriptionLabTestId)
        {
            var data = Repository.GetAll().Where(x => x.PrescriptionLabTestId == prescriptionLabTestId).ToList();
            var mapped = ObjectMapper.Map<List<CreateUpdateLabReportResultItemDto>>(data);
            return mapped;
        }
        public async Task EditLabReportResultItem(List<CreateUpdateLabReportResultItemDto> reportResultItemDtos)
        {
            try
            {
                if (reportResultItemDtos == null || reportResultItemDtos.Count == 0)
                    return;

                foreach (var dto in reportResultItemDtos)
                {
                    if (dto.Id > 0)
                    {
                        // Update existing record
                        var existingEntity = await Repository.GetAsync(dto.Id);
                        ObjectMapper.Map(dto, existingEntity); // This updates the properties
                        await Repository.UpdateAsync(existingEntity);
                    }
                    else
                    {
                        // Insert new record
                        var newEntity = ObjectMapper.Map<LabReportResultItem>(dto);
                        await Repository.InsertAsync(newEntity);
                    }
                }
                await CurrentUnitOfWork.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        [HttpGet]
        public async Task<ViewLabReportDto> ViewLabReport(long prescriptionLabTestId)
        {
            var data = _prescriptionLabTestsAppService.GetPrescriptionLabTestDetailsForViewReportById(prescriptionLabTestId);
            var newEntity = ObjectMapper.Map<ViewLabReportDto>(data);
            var tenant = await _tenantManager.GetByIdAsync(AbpSession.TenantId.Value);
            newEntity.TenantName = tenant?.Name;
            return newEntity;
        }
    }
}
