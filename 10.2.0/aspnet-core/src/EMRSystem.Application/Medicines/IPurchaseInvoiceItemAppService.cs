using Abp.Application.Services;
using Abp.Application.Services.Dto;
using EMRSystem.Medicines.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMRSystem.Medicines
{
    public interface IPurchaseInvoiceItemAppService :
        IAsyncCrudAppService<PurchaseInvoiceItemDto, long, PagedAndSortedResultRequestDto, CreateUpdatePurchaseInvoiceItemDto, CreateUpdatePurchaseInvoiceItemDto>
    { }
}
