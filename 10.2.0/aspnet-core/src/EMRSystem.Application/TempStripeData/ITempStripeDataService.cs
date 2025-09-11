using EMRSystem.Pharmacist.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMRSystem.TempStripeData
{
    public interface ITempStripeDataService
    {
        Task StoreData(string id, CreatePharmacistPrescriptionsWithItemDto data);
        Task<CreatePharmacistPrescriptionsWithItemDto> RetrieveData(string id);
        Task RemoveData(string id);
    }
}
