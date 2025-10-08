using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Domain.Repositories;
using Abp.Extensions;
using Abp.Linq.Extensions;
using EMRSystem.Medicines.Dto;
using EMRSystem.Pharmacist.Dto;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMRSystem.Medicines
{
    public class MedicineMasterAppService :
        AsyncCrudAppService<MedicineMaster, MedicineMasterDto, long, PagedMedicineListResultRequestDto, CreateUpdateMedicineMasterDto>,
        IMedicineMasterAppService
    {
        public MedicineMasterAppService(IRepository<MedicineMaster, long> repository)
            : base(repository)
        {
        }
        protected override IQueryable<MedicineMaster> CreateFilteredQuery(PagedMedicineListResultRequestDto input)
        {
            return Repository.GetAll()
                .Include(x => x.Form)
                .Include(x => x.StrengthUnit)
                .WhereIf(!input.Keyword.IsNullOrWhiteSpace(),
                    s => s.Form.Name.Contains(input.Keyword) ||
                         s.Name.Contains(input.Keyword)||
                         s.StrengthUnit.Name.Contains(input.Keyword));
        }

        // Map entity to DTO including names
        protected override MedicineMasterDto MapToEntityDto(MedicineMaster entity)
        {
            var dto = base.MapToEntityDto(entity);

            dto.FormName = entity.Form?.Name;
            dto.StrengthUnitName = entity.StrengthUnit?.Name;

            return dto;
        }
        public async Task<List<MedicineMasterDto>> GetAllListOfMedicine()
        {
            var list = await Repository.GetAll().Where(x => x.TenantId == AbpSession.TenantId).ToListAsync();
            var itemDtos = ObjectMapper.Map<List<MedicineMasterDto>>(list);
            return itemDtos;
        }
        public async Task<List<MedicineWithStockDto>> GetMedicinesWithStockAsync()
        {
            var tenantId = AbpSession.TenantId ?? 0;

            // Get all medicines including Stocks and related entities
            var medicines = await Repository.GetAll()
                .Include(x => x.Form)
                .Include(x => x.StrengthUnit)
                .Include(x => x.Stocks) // Include all stock batches
                .Where(x => x.TenantId == tenantId && x.IsAvailable)
                .ToListAsync();

            // Map to DTO
            var result = medicines.Select(m => new MedicineWithStockDto
            {
                Id = m.Id,
                MedicineName = m.Name,
                FormName = m.Form?.Name,
                Strength = m.Strength,
                StrengthUnitName = m.StrengthUnit?.Name,
                Stocks = m.Stocks
                    .Where(s => !s.IsExpire && s.Quantity > 0) // only available & non-expired stocks
                    .Select(s => new MedicineStockDto
                    {
                        BatchNo = s.BatchNo,
                        ExpiryDate = s.ExpiryDate,
                        Quantity = s.Quantity,
                        PurchasePrice = s.PurchasePrice,
                        SellingPrice = s.SellingPrice
                    }).ToList()
            }).ToList();

            return result;
        }

        public async Task<List<MedicineLookupDto>> GetMedicinesByFormIdAsync(long formId)
        {
            var tenantId = AbpSession.TenantId ?? 0;

            var query = Repository.GetAll()
                .Include(x => x.StrengthUnit) // strength unit fetch karna hai
                .Where(x => x.TenantId == tenantId && x.MedicineFormId == formId && x.IsAvailable);

            var medicines = await query.ToListAsync();

            var result = medicines.Select(m => new MedicineLookupDto
            {
                Id = m.Id,
                MedicineName = m.Name,
                MedicineFormId = m.MedicineFormId,
                DosageOption = m.Strength.HasValue? $"{m.Strength.Value.ToString("0.##")} {m.StrengthUnit.Name}": $"N/A {m.StrengthUnit.Name}"

            }).ToList();


            return result;
        }
        //public async Task<MedicineWithStockDto> GetMedicineWithStockByIdAsync(long medicineMasterId)
        //{
        //    var tenantId = AbpSession.TenantId ?? 0;

        //    // Find the specific medicine including its form, strength unit and stocks
        //    var medicine = await Repository.GetAll()
        //        .Include(x => x.Form)
        //        .Include(x => x.StrengthUnit)
        //        .Include(x => x.Stocks)
        //        .Where(x => x.TenantId == tenantId
        //                 && x.IsAvailable
        //                 && x.Id == medicineMasterId)
        //        .FirstOrDefaultAsync();

        //    if (medicine == null)
        //    {
        //        return null; // or throw exception if not found
        //    }

        //    // Map to DTO
        //    var result = new MedicineWithStockDto
        //    {
        //        Id = medicine.Id,
        //        MedicineName = medicine.Name,
        //        FormName = medicine.Form?.Name,
        //        Strength = medicine.Strength,
        //        StrengthUnitName = medicine.StrengthUnit?.Name,
        //        Stocks = medicine.Stocks
        //            .Where(s => !s.IsExpire && s.Quantity > 0) // only valid stocks
        //            .Select(s => new MedicineStockDto
        //            {
        //                Id = s.Id,
        //                TenantId = s.TenantId,
        //                MedicineMasterId = s.MedicineMasterId,
        //                MedicineName = medicine.Name,
        //                BatchNo = s.BatchNo,
        //                ExpiryDate = s.ExpiryDate,
        //                Quantity = s.Quantity,
        //                PurchasePrice = s.PurchasePrice,
        //                SellingPrice = s.SellingPrice,
        //                IsExpire = s.IsExpire,
        //                TotalStock = medicine.TotalAvailableQuantity,
        //                StockStatus = s.Status.ToString(),
        //                ExpiryStatus = (s.ExpiryDate.HasValue && s.ExpiryDate.Value.Date < DateTime.Today)
        //                               ? "Expired"
        //                               : "Valid"
        //            }).ToList()
        //    };

        //    return result;
        //}

        public async Task<MedicineWithStockDto> GetMedicineWithStockByIdAsync(long medicineMasterId)
        {
            var tenantId = AbpSession.TenantId ?? 0;

            var medicine = await Repository.GetAll()
                .Include(x => x.Form)
                .Include(x => x.StrengthUnit)
                .Include(x => x.Stocks)
                .Where(x => x.TenantId == tenantId
                         && x.IsAvailable
                         && x.Id == medicineMasterId)
                .FirstOrDefaultAsync();

            if (medicine == null)
            {
                return null; // ya UserFriendlyException throw kar sakte ho
            }

            var result = new MedicineWithStockDto
            {
                Id = medicine.Id,
                MedicineName = medicine.Name,
                FormName = medicine.Form?.Name,
                Strength = medicine.Strength,
                StrengthUnitName = medicine.StrengthUnit?.Name,
                Stocks = medicine.Stocks
                    .Where(s => s.Quantity > 0) // sirf qty > 0 stocks
                    .Select(s => new MedicineStockDto
                    {
                        Id = s.Id,
                        TenantId = s.TenantId,
                        MedicineMasterId = s.MedicineMasterId,
                        MedicineName = medicine.Name,
                        BatchNo = s.BatchNo,
                        ExpiryDate = s.ExpiryDate,
                        Quantity = s.Quantity,
                        PurchasePrice = s.PurchasePrice,
                        SellingPrice = s.SellingPrice,
                        IsExpire = s.IsExpire,
                        TotalStock = medicine.TotalAvailableQuantity,
                        StockStatus = s.Status.ToString(),
                        ExpiryStatus = GetExpiryStatus(s.ExpiryDate),
                        DaysToExpire = GetDaysToExpire(s.ExpiryDate)
                    }).ToList()
            };

            return result;
        }

        // Helper Method for Expiry Status
        private string GetExpiryStatus(DateTime? expiryDate)
        {
            if (!expiryDate.HasValue)
                return "Valid";

            var today = DateTime.Today;
            var daysToExpire = (expiryDate.Value.Date - today).TotalDays;

            if (daysToExpire < 0)
                return "Expired";

            if (daysToExpire <= 30)
                return "Near Expiry";

            return "Valid";
        }

        // ✅ Helper Method for exact days
        private int? GetDaysToExpire(DateTime? expiryDate)
        {
            if (!expiryDate.HasValue)
                return null;

            var today = DateTime.Today;
            return (int)(expiryDate.Value.Date - today).TotalDays;
        }


    }

}
