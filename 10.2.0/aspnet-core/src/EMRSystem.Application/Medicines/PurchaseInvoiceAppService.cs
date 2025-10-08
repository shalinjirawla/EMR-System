using Abp;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.EntityFrameworkCore.Repositories;
using Abp.Extensions;
using Abp.Linq.Extensions;
using Abp.UI;
using EFCore.BulkExtensions;
using EMRSystem.Medicines.Dto;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EMRSystem.Medicines
{
    public class PurchaseInvoiceAppService :
        AsyncCrudAppService<PurchaseInvoice, PurchaseInvoiceDto, long, PagedPurchaseMedicineDto, CreateUpdatePurchaseInvoiceDto, CreateUpdatePurchaseInvoiceDto>,
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
        protected override IQueryable<PurchaseInvoice> CreateFilteredQuery(PagedPurchaseMedicineDto input)
        {
            return Repository.GetAll()
                .WhereIf(!input.Keyword.IsNullOrWhiteSpace(),
                    s => s.SupplierName.Contains(input.Keyword) ||
                         s.InvoiceNo.Contains(input.Keyword)); // if you have Medicine Code
        }
        public override async Task<PurchaseInvoiceDto> GetAsync(EntityDto<long> input)
        {
            var invoice = await Repository.GetAll()
                .Include(i => i.Items)
                .ThenInclude(item => item.MedicineMaster)
                .FirstOrDefaultAsync(i => i.Id == input.Id);

            if (invoice == null)
                throw new AbpException("Invoice not found");

            var invoiceDto = ObjectMapper.Map<PurchaseInvoiceDto>(invoice);

            return invoiceDto;
        }

        [UnitOfWork]
        public override async Task<PurchaseInvoiceDto> CreateAsync(CreateUpdatePurchaseInvoiceDto input)
        {
            try { 
            
           
            var invoice = ObjectMapper.Map<PurchaseInvoice>(input);
            await Repository.InsertAsync(invoice);
            await CurrentUnitOfWork.SaveChangesAsync();

            var distinctInvoiceItems = input.Items
                .GroupBy(x => new
                {
                    x.MedicineMasterId,
                    x.BatchNo,
                    ExpiryDate = x.ExpiryDate?.Date,
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

            var existingStocks = _stockRepository.GetAll()
                     .ToList() 
                     .Join(
                         distinctInvoiceItems, 
                         stock => new { stock.MedicineMasterId, stock.BatchNo, stock.ExpiryDate, stock.SellingPrice },
                         item => new { item.MedicineMasterId, item.BatchNo, item.ExpiryDate, item.SellingPrice },
                         (stock, item) => stock
                     )
                     .ToList();

            var stockDict = existingStocks.ToDictionary(
                s => (s.MedicineMasterId, s.BatchNo, s.ExpiryDate, s.SellingPrice),
                s => s
            );

            var newStocks = new List<MedicineStock>();
            var updateStocks = new List<MedicineStock>();

            // 4️⃣ Split into update/new
            foreach (var item in distinctInvoiceItems)
            {
                var key = (item.MedicineMasterId, item.BatchNo, item.ExpiryDate, item.SellingPrice);

                if (stockDict.TryGetValue(key, out var stock))
                {
                    stock.Quantity += item.Quantity;
                    updateStocks.Add(stock);
                }
                else
                {
                    newStocks.Add(new MedicineStock
                    {
                        TenantId = item.TenantId,
                        MedicineMasterId = item.MedicineMasterId,
                        BatchNo = item.BatchNo,
                        ExpiryDate = item.ExpiryDate,
                        Quantity = item.Quantity,
                        PurchasePrice = item.PurchasePrice,
                        SellingPrice = item.SellingPrice,
                        IsExpire = false
                    });
                }
            }

                var db = _stockRepository.GetDbContext();

                if (updateStocks.Any())
                    await db.BulkUpdateAsync(updateStocks);

                if (newStocks.Any())
                    await db.BulkInsertAsync(newStocks);

                await CurrentUnitOfWork.SaveChangesAsync();

                return MapToEntityDto(invoice);

            }
            catch (Exception ex)
            {
                Logger.Error("Error while creating Purchase Invoice", ex);
                throw new UserFriendlyException("Invoice creation failed. Please try again.");
            }
        }

        [UnitOfWork]
        public override async Task<PurchaseInvoiceDto> UpdateAsync(CreateUpdatePurchaseInvoiceDto input)
        {
            var invoice = await Repository.GetAllIncluding(i => i.Items)
                .FirstOrDefaultAsync(i => i.Id == input.Id);

            if (invoice == null)
                throw new AbpException("Invoice not found");

            foreach (var oldItem in invoice.Items.ToList()) 
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
                        _stockRepository.Delete(stock);
                    }
                    else
                    {
                        _stockRepository.Update(stock);
                    }
                }

                _invoiceItemRepository.Delete(oldItem);
            }

            invoice.InvoiceNo = input.InvoiceNo;
            invoice.InvoiceDate = input.InvoiceDate;
            invoice.SupplierName = input.SupplierName;
            invoice.TotalAmount = input.TotalAmount;

            Repository.Update(invoice); 

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

            await CurrentUnitOfWork.SaveChangesAsync();

            return MapToEntityDto(invoice);
        }

        //[UnitOfWork]
        //public override async Task<PurchaseInvoiceDto> CreateAsync(CreateUpdatePurchaseInvoiceDto input)
        //{
        //    // 1️⃣ Invoice save karo
        //    var invoice = ObjectMapper.Map<PurchaseInvoice>(input);
        //    await Repository.InsertAsync(invoice);
        //    await CurrentUnitOfWork.SaveChangesAsync(); // invoice.Id chahiye

        //    // 2️⃣ Distinct InvoiceItems
        //    var distinctInvoiceItems = input.Items
        //         .GroupBy(x => new
        //         {
        //             x.MedicineMasterId,
        //             x.BatchNo,
        //             ExpiryDate = x.ExpiryDate.HasValue ? x.ExpiryDate.Value.Date : (DateTime?)null,
        //             x.SellingPrice
        //         })
        //         .Select(g => new CreateUpdatePurchaseInvoiceItemDto
        //         {
        //             TenantId = g.First().TenantId,
        //             MedicineMasterId = g.Key.MedicineMasterId,
        //             BatchNo = g.Key.BatchNo,
        //             ExpiryDate = g.Key.ExpiryDate,
        //             Quantity = g.Sum(x => x.Quantity),
        //             PurchasePrice = g.First().PurchasePrice,
        //             SellingPrice = g.Key.SellingPrice
        //         }).ToList();

        //    foreach (var item in distinctInvoiceItems)
        //    {

        //        // Stock merge
        //        var existingStock = await _stockRepository.GetAll()
        //            .Where(s =>
        //                s.TenantId == item.TenantId &&
        //                s.MedicineMasterId == item.MedicineMasterId &&
        //                s.BatchNo == item.BatchNo &&
        //                s.ExpiryDate == item.ExpiryDate &&
        //                s.SellingPrice == item.SellingPrice
        //            )
        //            .FirstOrDefaultAsync();

        //        if (existingStock != null)
        //        {
        //            existingStock.Quantity += item.Quantity;
        //            await _stockRepository.UpdateAsync(existingStock);
        //        }
        //        else
        //        {
        //            var newStock = new MedicineStock
        //            {
        //                TenantId = item.TenantId,
        //                MedicineMasterId = item.MedicineMasterId,
        //                BatchNo = item.BatchNo,
        //                ExpiryDate = item.ExpiryDate,
        //                Quantity = item.Quantity,
        //                PurchasePrice = item.PurchasePrice,
        //                SellingPrice = item.SellingPrice,
        //                IsExpire = false
        //            };
        //            await _stockRepository.InsertAsync(newStock);
        //        }
        //    }

        //    await CurrentUnitOfWork.SaveChangesAsync();
        //    return MapToEntityDto(invoice);
        //}


    }
}
