using Abp.Application.Services.Dto;
using Abp.Application.Services;
using Abp.Domain.Repositories;
using EMRSystem.Pharmacist.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Abp.UI;

namespace EMRSystem.Pharmacist
{

    public class PharmacistPrescriptionsAppService : AsyncCrudAppService<EMRSystem.Pharmacists.PharmacistPrescriptions, PharmacistPrescriptionsDto, long, PagedAndSortedResultRequestDto, CreateUpdatePharmacistPrescriptionsDto, CreateUpdatePharmacistPrescriptionsDto>,
     IPharmacistPrescriptionsAppService
    {
        public PharmacistPrescriptionsAppService(IRepository<EMRSystem.Pharmacists.PharmacistPrescriptions, long> repository) : base(repository)
        {
        }

        [HttpGet]
        public async Task<PagedResultDto<PharmacistPrescriptionsDto>> GetPrescriptionFulfillment(PagedAndSortedResultRequestDto input)
        {
            try
            {
                var list = await Repository.GetAllIncluding
                        (
                        x => x.Prescriptions,
                        x => x.Prescriptions.Patient,
                        x => x.Prescriptions.Doctor
                        )
            .Where(x => x.Prescriptions != null && x.Prescriptions.Patient != null && x.Prescriptions.Patient.IsAdmitted).ToListAsync();
                var mappedItems = ObjectMapper.Map<List<PharmacistPrescriptionsDto>>(list);
                return new PagedResultDto<PharmacistPrescriptionsDto>(
                    list.Count,
                    mappedItems
                );
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException("Error in GetPrescriptionFulfillment", ex);
            }
        }

        public async Task CreatePharmacistPrescriptionsWithItem(CreateUpdatePharmacistPrescriptionsDto createUpdate)
        {
            var mappedItem = ObjectMapper.Map<EMRSystem.Pharmacists.PharmacistPrescriptions>(createUpdate);
            var res = await Repository.InsertAndGetIdAsync(mappedItem);
        }
    }
}
