using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using EMRSystem.Medicines.Dto;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace EMRSystem.Medicines
{
    public class PurchaseInvoiceAppService :
        AsyncCrudAppService<PurchaseInvoice, PurchaseInvoiceDto, long, PagedAndSortedResultRequestDto, CreateUpdatePurchaseInvoiceDto, CreateUpdatePurchaseInvoiceDto>,
        IPurchaseInvoiceAppService
    {
        private readonly IRepository<PurchaseInvoiceItem, long> _invoiceItemRepository;
        private readonly IRepository<MedicineStock, long> _stockRepository;

        public PurchaseInvoiceAppService(
            IRepository<PurchaseInvoice, long> repository,
            IRepository<PurchaseInvoiceItem, long> invoiceItemRepository,
            IRepository<MedicineStock, long> stockRepository)
            : base(repository)
        {
            _invoiceItemRepository = invoiceItemRepository;
            _stockRepository = stockRepository;
        }

        [UnitOfWork]
        public override async Task<PurchaseInvoiceDto> CreateAsync(CreateUpdatePurchaseInvoiceDto input)
        {
            // 1️⃣ Invoice save karo
            var invoice = ObjectMapper.Map<PurchaseInvoice>(input);
            await Repository.InsertAsync(invoice);
            await CurrentUnitOfWork.SaveChangesAsync(); // invoice.Id chahiye

            // 2️⃣ Distinct InvoiceItems
            var distinctInvoiceItems = input.Items
                 .GroupBy(x => new
                 {
                     x.MedicineMasterId,
                     x.BatchNo,
                     ExpiryDate = x.ExpiryDate.HasValue ? x.ExpiryDate.Value.Date : (DateTime?)null,
                     x.SellingPrice
                 })
                 .Select(g => new CreateUpdatePurchaseInvoiceItemDto
                 {
                     TenantId = g.First().TenantId,
                     MedicineMasterId = g.Key.MedicineMasterId,
                     BatchNo = g.Key.BatchNo,
                     ExpiryDate = g.Key.ExpiryDate,
                     Quantity = g.Sum(x => x.Quantity),
                     PurchasePrice = g.First().PurchasePrice,
                     SellingPrice = g.Key.SellingPrice
                 }).ToList();

            foreach (var item in distinctInvoiceItems)
            {
                // InvoiceItem insert
                var invoiceItem = new PurchaseInvoiceItem
                {
                    TenantId = item.TenantId,
                    PurchaseInvoiceId = invoice.Id,
                    MedicineMasterId = item.MedicineMasterId,
                    BatchNo = item.BatchNo,
                    ExpiryDate = item.ExpiryDate,
                    Quantity = item.Quantity,
                    PurchasePrice = item.PurchasePrice,
                    SellingPrice = item.SellingPrice
                };
                await _invoiceItemRepository.InsertAsync(invoiceItem);

                // Stock merge
                var existingStock = await _stockRepository.GetAll()
                    .Where(s =>
                        s.TenantId == item.TenantId &&
                        s.MedicineMasterId == item.MedicineMasterId &&
                        s.BatchNo == item.BatchNo &&
                        s.ExpiryDate == item.ExpiryDate &&
                        s.SellingPrice == item.SellingPrice
                    )
                    .FirstOrDefaultAsync();

                if (existingStock != null)
                {
                    existingStock.Quantity += item.Quantity;
                    await _stockRepository.UpdateAsync(existingStock);
                }
                else
                {
                    var newStock = new MedicineStock
                    {
                        TenantId = item.TenantId,
                        MedicineMasterId = item.MedicineMasterId,
                        BatchNo = item.BatchNo,
                        ExpiryDate = item.ExpiryDate,
                        Quantity = item.Quantity,
                        PurchasePrice = item.PurchasePrice,
                        SellingPrice = item.SellingPrice,
                        IsExpire = false
                    };
                    await _stockRepository.InsertAsync(newStock);
                }
            }

            await CurrentUnitOfWork.SaveChangesAsync();
            return MapToEntityDto(invoice);
        }


    }
}
