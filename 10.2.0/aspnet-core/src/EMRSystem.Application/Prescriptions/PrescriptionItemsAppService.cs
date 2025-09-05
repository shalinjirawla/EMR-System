using Abp.Application.Services.Dto;
using Abp.Application.Services;
using Abp.Domain.Repositories;
using EMRSystem.Prescriptions.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Abp.EntityFrameworkCore.Repositories;

namespace EMRSystem.Prescriptions
{
    public class PrescriptionItemsAppService : AsyncCrudAppService<PrescriptionItem, PrescriptionItemDto, long, PagedAndSortedResultRequestDto, CreateUpdatePrescriptionItemDto, CreateUpdatePrescriptionItemDto>,
   IPrescriptionItemsAppService
    {
        public PrescriptionItemsAppService(IRepository<PrescriptionItem, long> repository) : base(repository)
        {
        }
        public async Task CreatePrescriptionItemList(List<PharmacistPrescriptionItemWithUnitPriceDto> listOfItem)
        {
            try
            {
                var entity = ObjectMapper.Map<List<PrescriptionItem>>(listOfItem);
                var prescriptionID = listOfItem[0].PrescriptionId;
                var getPrescriptionItem = await Repository.GetAll().Where(x => x.PrescriptionId == prescriptionID).ToListAsync();
                if (getPrescriptionItem.Count > 0)
                {
                    Repository.RemoveRange(getPrescriptionItem);
                    await Repository.InsertRangeAsync(entity);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
