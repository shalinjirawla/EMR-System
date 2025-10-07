using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Domain.Repositories;
using Abp.Extensions;
using Abp.Linq.Extensions;
using Abp.UI;
using EMRSystem.Medicines;
using EMRSystem.Medicines.Dto;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMRSystem.Medicines
{
    public class MedicineStockAppService :
        AsyncCrudAppService<MedicineStock, MedicineStockDto, long, PagedMedicineStockResultRequestDto, CreateUpdateMedicineStockDto>,
        IMedicineStockAppService
    {
        public MedicineStockAppService(IRepository<MedicineStock, long> repository)
            : base(repository)
        {

        }
        protected override IQueryable<MedicineStock> CreateFilteredQuery(PagedMedicineStockResultRequestDto input)
        {
            return Repository.GetAll()
                .Include(s => s.MedicineMaster)
                .WhereIf(!input.Keyword.IsNullOrWhiteSpace(),
                    s => s.MedicineMaster.Name.Contains(input.Keyword) ||
                         s.BatchNo.Contains(input.Keyword)); // if you have Medicine Code
        }

        protected override MedicineStockDto MapToEntityDto(MedicineStock entity)
        {
            var dto = new MedicineStockDto
            {
                Id = entity.Id,
                TenantId = entity.TenantId,
                MedicineMasterId = entity.MedicineMasterId,
                MedicineName = entity.MedicineMaster?.Name,
                BatchNo = entity.BatchNo,
                ExpiryDate = entity.ExpiryDate,
                Quantity = entity.Quantity,
                PurchasePrice = entity.PurchasePrice,
                SellingPrice = entity.SellingPrice,
                IsExpire = entity.IsExpire
            };

            // Stock status
            dto.StockStatus = entity.Quantity <= 0 ? "Out of Stock" :
                              entity.MedicineMaster != null && entity.Quantity <= entity.MedicineMaster.MinimumStock ? "Low Stock" :
                              "In Stock";

            if (!entity.ExpiryDate.HasValue)
            {
                dto.ExpiryStatus = "No Expiry Date";
            }
            else
            {
                var expiryDate = entity.ExpiryDate.Value;
                var daysToExpire = (expiryDate - DateTime.Today).TotalDays;

                dto.ExpiryStatus = expiryDate < DateTime.Today ? "Expired" :
                                   daysToExpire <= 30 ? "Expires Soon" : "Valid";
            }

            return dto;
        }

        public async Task<List<AllocateMedicineResultDto>> AllocateMedicineAsync(long medicineId, int requestedQty)
        {
            var allocatedList = new List<AllocateMedicineResultDto>();

            var stocks = await Repository.GetAll()
                .Where(s => s.MedicineMasterId == medicineId
                         && !s.IsExpire
                         && (s.ExpiryDate == null || s.ExpiryDate >= DateTime.Today)
                         && s.Quantity > 0)
                .OrderBy(s => s.ExpiryDate)
                .ToListAsync();

            int remaining = requestedQty;

            foreach (var stock in stocks)
            {
                if (remaining <= 0)
                    break;

                int takeQty = Math.Min(stock.Quantity, remaining);

                allocatedList.Add(new AllocateMedicineResultDto
                {
                    MedicineId = medicineId,
                    MedicineName = stock.MedicineMaster.Name,
                    BatchNo = stock.BatchNo,
                    Quantity = takeQty,
                    Price = takeQty * stock.SellingPrice,
                    ExpiryDate = stock.ExpiryDate
                });

                remaining -= takeQty;
            }

            if (remaining > 0)
                throw new UserFriendlyException($"Ye stock me sirf {requestedQty - remaining} qty available hai!");

            return allocatedList;
        }


    }
}
