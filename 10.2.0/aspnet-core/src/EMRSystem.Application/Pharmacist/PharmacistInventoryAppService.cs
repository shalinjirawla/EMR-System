using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.Extensions;
using Abp.Linq.Extensions;
using EMRSystem.Authorization;
using EMRSystem.Pharmacist.Dto;
using EMRSystem.Pharmacists;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Dynamic.Core; 

namespace EMRSystem.Pharmacist
{
    //[AbpAuthorize(PermissionNames.Pages_Pharmacist)]
    public class PharmacistInventoryAppService : AsyncCrudAppService<EMRSystem.Pharmacists.PharmacistInventory, PharmacistInventoryDto, long, PagedPharmacistInventoryResultRequestDto, CreateUpdatePharmacistInventoryDto, CreateUpdatePharmacistInventoryDto>,
    IPharmacistInventoryAppService
    {
        public PharmacistInventoryAppService(IRepository<EMRSystem.Pharmacists.PharmacistInventory, long> repository) : base(repository)
        {
        }
        protected override IQueryable<PharmacistInventory> CreateFilteredQuery(PagedPharmacistInventoryResultRequestDto input)
        {
            return Repository.GetAll()
                .WhereIf(!input.Keyword.IsNullOrWhiteSpace(), x =>
                    x.MedicineName.Contains(input.Keyword) ||
                    x.Description.Contains(input.Keyword) ||
                    x.Unit.Contains(input.Keyword))
                .WhereIf(input.Stock.HasValue, x => x.Stock == input.Stock.Value)
                .WhereIf(input.FromExpiryDate.HasValue, x => x.ExpiryDate >= input.FromExpiryDate.Value)
                .WhereIf(input.ToExpiryDate.HasValue, x => x.ExpiryDate <= input.ToExpiryDate.Value)
                .WhereIf(input.IsAvailable.HasValue, x => x.IsAvailable == input.IsAvailable.Value)
                .Select(x => new PharmacistInventory
                {
                    Id = x.Id,
                    TenantId = x.TenantId,
                    MedicineName = x.MedicineName,
                    CostPrice = x.CostPrice,
                    SellingPrice = x.SellingPrice,
                    ExpiryDate = x.ExpiryDate,
                    PurchaseDate = x.PurchaseDate,
                    Unit = x.Unit,
                    Stock = x.Stock,
                    MinStock = x.MinStock,
                    Description = x.Description,
                    IsAvailable = x.IsAvailable
                });
        }

        protected override IQueryable<PharmacistInventory> ApplySorting(IQueryable<PharmacistInventory> query, PagedPharmacistInventoryResultRequestDto input)
        {
            if (!string.IsNullOrWhiteSpace(input.Sorting))
            {
                return query.OrderBy(input.Sorting);
            }

            return query.OrderBy(x => x.MedicineName);
        }

        protected override async Task<PharmacistInventory> GetEntityByIdAsync(long id)
        {
            return await Repository.GetAll()
                .Where(x => x.Id == id)
                .FirstOrDefaultAsync();
        }

        protected override PharmacistInventoryDto MapToEntityDto(PharmacistInventory entity)
        {
            var dto = base.MapToEntityDto(entity);

            // Calculate statuses when mapping to DTO
            dto.StockStatus = entity.Stock <= 0 ? "Out of Stock" :
                            entity.Stock <= entity.MinStock ? "Low Stock" : "In Stock";
            dto.ExpiryStatus = entity.ExpiryDate < DateTime.Now ? "Expired" :
                             (entity.ExpiryDate - DateTime.Now).TotalDays <= 30 ? "Expires Soon" : "Valid";

            return dto;
        }
        // PharmacistInventoryAppService.cs
        public async Task<PagedResultDto<PharmacistInventoryDto>> GetAllByTenantIdAsync(int tenantId)
        {
            // Get query with all medicines for the tenant
            var query = Repository.GetAll()
                .Where(x => x.TenantId == tenantId)
                .OrderBy(x => x.MedicineName);

            // Get total count
            var totalCount = await query.CountAsync();

            // Get items
            var items = await query.ToListAsync();

            // Map to DTOs
            var itemDtos = ObjectMapper.Map<List<PharmacistInventoryDto>>(items);

            return new PagedResultDto<PharmacistInventoryDto>(totalCount, itemDtos);
        }
    }
}
