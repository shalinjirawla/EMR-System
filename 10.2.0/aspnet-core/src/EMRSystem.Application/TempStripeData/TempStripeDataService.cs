using EMRSystem.Pharmacist.Dto;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMRSystem.TempStripeData
{
    public class TempStripeDataService : ITempStripeDataService
    {
        private readonly IDistributedCache _cache;

        public TempStripeDataService(IDistributedCache cache)
        {
            _cache = cache;
        }

        public async Task StoreData(string id, CreatePharmacistPrescriptionsWithItemDto data)
        {
            var serialized = JsonConvert.SerializeObject(data);
            await _cache.SetStringAsync(id, serialized, new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5) // 30 minutes expiry
            });
        }

        public async Task<CreatePharmacistPrescriptionsWithItemDto> RetrieveData(string id)
        {
            var data = await _cache.GetStringAsync(id);
            return data == null ? null : JsonConvert.DeserializeObject<CreatePharmacistPrescriptionsWithItemDto>(data);
        }

        public async Task RemoveData(string id)
        {
            await _cache.RemoveAsync(id);
        }
    }
}
