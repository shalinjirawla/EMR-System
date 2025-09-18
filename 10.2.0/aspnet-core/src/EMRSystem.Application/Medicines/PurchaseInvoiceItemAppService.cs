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
    public class PurchaseInvoiceItemAppService :
        AsyncCrudAppService<PurchaseInvoiceItem, PurchaseInvoiceItemDto, long, PagedAndSortedResultRequestDto, CreateUpdatePurchaseInvoiceItemDto, CreateUpdatePurchaseInvoiceItemDto>,
        IPurchaseInvoiceItemAppService
    {
        public PurchaseInvoiceItemAppService(IRepository<PurchaseInvoiceItem, long> repository)
            : base(repository)
        {
        }
    }
}
