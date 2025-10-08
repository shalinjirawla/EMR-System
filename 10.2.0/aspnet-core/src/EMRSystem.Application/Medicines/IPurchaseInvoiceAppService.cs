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
    public interface IPurchaseInvoiceAppService :
        IAsyncCrudAppService<PurchaseInvoiceDto, long, PagedPurchaseMedicineDto, CreateUpdatePurchaseInvoiceDto, CreateUpdatePurchaseInvoiceDto>
    { }
}
