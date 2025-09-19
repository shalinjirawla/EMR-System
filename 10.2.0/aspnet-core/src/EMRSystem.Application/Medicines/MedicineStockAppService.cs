using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Domain.Repositories;
using EMRSystem.Medicines.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMRSystem.Medicines
{
    public class MedicineStockAppService :
        AsyncCrudAppService<MedicineStock, MedicineStockDto, long, PagedAndSortedResultRequestDto, CreateUpdateMedicineStockDto>,
        IMedicineStockAppService
    {
        public MedicineStockAppService(IRepository<MedicineStock, long> repository)
            : base(repository)
        {

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

            // Expiry status using nearest batch
            var nearestExpiry = Repository.GetAll()
                .Where(s => s.MedicineMasterId == entity.MedicineMasterId
                            && !s.IsExpire
                            && s.ExpiryDate.HasValue
                            && s.ExpiryDate.Value >= DateTime.Today)
                .OrderBy(s => s.ExpiryDate)
                .Select(s => s.ExpiryDate.Value)
                .FirstOrDefault();

            if (nearestExpiry == default)
            {
                dto.ExpiryStatus = "No Active Batch";
            }
            else
            {
                var daysToExpire = (nearestExpiry - DateTime.Now).TotalDays;
                dto.ExpiryStatus = nearestExpiry < DateTime.Now ? "Expired" :
                                   daysToExpire <= 30 ? "Expires Soon" : "Valid";
            }

            return dto;
        }


    }
}
