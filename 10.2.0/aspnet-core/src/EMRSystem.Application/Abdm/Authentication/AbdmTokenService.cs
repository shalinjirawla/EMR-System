using Abp.Dependency;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace EMRSystem.Abdm.Authentication
{
    public class AbdmTokenService : IAbdmTokenService, ITransientDependency
    {
        private readonly IMemoryCache _cache;
        private readonly IConfiguration _configuration;
        private readonly IHttpClientFactory _httpClientFactory;
        private const string TokenCacheKey = "AbdmAccessToken";

        public AbdmTokenService(IMemoryCache cache, IConfiguration configuration, IHttpClientFactory httpClientFactory)
        {
            _cache = cache;
            _configuration = configuration;
            _httpClientFactory = httpClientFactory;
        }

        public async Task<string> GetAccessTokenAsync()
        {
            if (_cache.TryGetValue(TokenCacheKey, out string cachedToken))
            {
                return cachedToken;
            }

            // Fetch from Gateway
            var clientId = _configuration["Abdm:ClientId"];
            var clientSecret = _configuration["Abdm:ClientSecret"];
            var gatewayUrl = _configuration["Abdm:GatewayUrl"]; 

            var client = _httpClientFactory.CreateClient("AbdmGateway");
            var requestBody = new
            {
                clientId = clientId,
                clientSecret = clientSecret,
                grantType = "client_credentials"
            };

            var content = new StringContent(JsonConvert.SerializeObject(requestBody), Encoding.UTF8, "application/json");

            var requestId = Guid.NewGuid().ToString();
            var timestamp = DateTime.UtcNow.ToString("o");

            client.DefaultRequestHeaders.Add("X-CM-ID", "sbx");
            client.DefaultRequestHeaders.Add("REQUEST-ID", requestId);
            client.DefaultRequestHeaders.Add("TIMESTAMP", timestamp);

            var baseUrl = gatewayUrl.EndsWith("/gateway") ? gatewayUrl.Substring(0, gatewayUrl.Length - 8) : gatewayUrl;
            var response = await client.PostAsync($"{baseUrl}/api/hiecm/gateway/v3/sessions", content);
            response.EnsureSuccessStatusCode();

            var responseString = await response.Content.ReadAsStringAsync();
            var tokenResponse = JsonConvert.DeserializeObject<AbdmTokenResponse>(responseString);

            // Cache token (expire 5 minutes before actual expiration)
            var cacheOptions = new MemoryCacheEntryOptions()
                .SetAbsoluteExpiration(TimeSpan.FromSeconds(tokenResponse.expiresIn - 300));

            _cache.Set(TokenCacheKey, tokenResponse.accessToken, cacheOptions);

            return tokenResponse.accessToken;
        }

        private class AbdmTokenResponse
        {
            public string accessToken { get; set; }
            public int expiresIn { get; set; }
            public string tokenType { get; set; }
        }
    }
}
