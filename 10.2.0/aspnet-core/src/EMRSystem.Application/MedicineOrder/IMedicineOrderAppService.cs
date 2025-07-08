using Abp.Application.Services;
using Abp.Application.Services.Dto;
using EMRSystem.MedicineOrder.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMRSystem.MedicineOrder
{
    public interface IMedicineOrderAppService : IAsyncCrudAppService<
         MedicineOrderDto, long, PagedAndSortedResultRequestDto,
         CreateUpdateMedicineOrderDto, CreateUpdateMedicineOrderDto>
    {}
}
