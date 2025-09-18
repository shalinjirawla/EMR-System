using Abp;
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
        public override async Task<PurchaseInvoiceDto> GetAsync(EntityDto<long> input)
        {
            // Invoice fetch with items and MedicineMaster included
            var invoice = await Repository.GetAll()
                .Include(i => i.Items)
                .ThenInclude(item => item.MedicineMaster)  // nested property
                .FirstOrDefaultAsync(i => i.Id == input.Id);

            if (invoice == null)
                throw new AbpException("Invoice not found");

            // AutoMapper se mapping
            var invoiceDto = ObjectMapper.Map<PurchaseInvoiceDto>(invoice);

            return invoiceDto;
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
        [UnitOfWork]
        public override async Task<PurchaseInvoiceDto> UpdateAsync(CreateUpdatePurchaseInvoiceDto input)
        {
            // 1️⃣ Existing invoice fetch karo with items
            var invoice = await Repository.GetAllIncluding(i => i.Items)
                .FirstOrDefaultAsync(i => i.Id == input.Id);

            if (invoice == null)
                throw new AbpException("Invoice not found");

            // 2️⃣ Old items rollback karo stock me
            foreach (var oldItem in invoice.Items.ToList()) // ToList to avoid collection modification issues
            {
                var stock = await _stockRepository
                    .GetAll()
                    .FirstOrDefaultAsync(s =>
                        s.TenantId == oldItem.TenantId &&
                        s.MedicineMasterId == oldItem.MedicineMasterId &&
                        s.BatchNo == oldItem.BatchNo &&
                        s.ExpiryDate.HasValue && oldItem.ExpiryDate.HasValue &&
                        s.ExpiryDate.Value.Date == oldItem.ExpiryDate.Value.Date &&
                        s.SellingPrice == oldItem.SellingPrice
                    );

                if (stock != null)
                {
                    stock.Quantity -= oldItem.Quantity;
                    if (stock.Quantity <= 0)
                    {
                        // EF will track this deletion
                        _stockRepository.Delete(stock);
                    }
                    else
                    {
                        // EF will track this update
                        _stockRepository.Update(stock);
                    }
                }

                _invoiceItemRepository.Delete(oldItem);
            }

            // 3️⃣ Map invoice properties from input (without items)
            invoice.InvoiceNo = input.InvoiceNo;
            invoice.InvoiceDate = input.InvoiceDate;
            invoice.SupplierName = input.SupplierName;
            invoice.TotalAmount = input.TotalAmount;

            Repository.Update(invoice); // EF tracks this

            // 4️⃣ Group new items to merge duplicates
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
                })
                .ToList();

            // 5️⃣ Add new items and update/insert stock
            foreach (var item in distinctInvoiceItems)
            {
                var invoiceItem = ObjectMapper.Map<PurchaseInvoiceItem>(item);
                invoiceItem.PurchaseInvoiceId = invoice.Id;
                _invoiceItemRepository.Insert(invoiceItem);

                var existingStock = await _stockRepository
                    .GetAll()
                    .Where(s =>
                        s.TenantId == item.TenantId &&
                        s.MedicineMasterId == item.MedicineMasterId &&
                        s.BatchNo == item.BatchNo &&
                        s.ExpiryDate.HasValue && item.ExpiryDate.HasValue &&
                        s.ExpiryDate.Value.Date == item.ExpiryDate.Value.Date &&
                        s.SellingPrice == item.SellingPrice
                    )
                    .FirstOrDefaultAsync();

                if (existingStock != null)
                {
                    existingStock.Quantity += item.Quantity;
                    _stockRepository.Update(existingStock);
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
                    _stockRepository.Insert(newStock);
                }
            }

            // 6️⃣ Save everything in **single SaveChangesAsync** to avoid concurrency errors
            await CurrentUnitOfWork.SaveChangesAsync();

            return MapToEntityDto(invoice);
        }

    }
}
