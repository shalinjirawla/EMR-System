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
    }
}
