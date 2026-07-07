using Abp.Dependency;
using EMRSystem.Configuration;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace EMRSystem.Abdm.Sessions
{
    public class AbdmSessionManager : IAbdmSessionManager, ISingletonDependency
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IMemoryCache _memoryCache;
        private readonly AbhaConfig _abhaConfig;
        
        private const string TokenCacheKey = "AbdmGatewayToken";

        public AbdmSessionManager(
            IHttpClientFactory httpClientFactory, 
            IMemoryCache memoryCache, 
            IOptions<AbhaConfig> abhaConfigOptions)
        {
            _httpClientFactory = httpClientFactory;
            _memoryCache = memoryCache;
            _abhaConfig = abhaConfigOptions.Value;
        }

        public async Task<string> GetAccessTokenAsync()
        {
            if (_memoryCache.TryGetValue(TokenCacheKey, out string cachedToken))
            {
                return cachedToken;
            }

            var client = _httpClientFactory.CreateClient();
            var requestBody = new
            {
                clientId = _abhaConfig.ClientId,
                clientSecret = _abhaConfig.ClientSecret
            };

            var content = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");
            
            var response = await client.PostAsync($"{_abhaConfig.GatewayUrl.TrimEnd('/')}/v0.5/sessions", content);
            response.EnsureSuccessStatusCode();

            var responseString = await response.Content.ReadAsStringAsync();
            var tokenResponse = JsonSerializer.Deserialize<GatewayTokenResponse>(responseString, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (string.IsNullOrEmpty(tokenResponse?.AccessToken))
            {
                throw new Exception("Failed to retrieve access token from ABDM gateway.");
            }

            // Cache token until it expires (subtracting a buffer of 1 minute)
            var cacheEntryOptions = new MemoryCacheEntryOptions()
                .SetAbsoluteExpiration(TimeSpan.FromSeconds(tokenResponse.ExpiresIn - 60));

            _memoryCache.Set(TokenCacheKey, tokenResponse.AccessToken, cacheEntryOptions);

            return tokenResponse.AccessToken;
        }

        private class GatewayTokenResponse
        {
            public string AccessToken { get; set; }
            public int ExpiresIn { get; set; }
            public int RefreshExpiresIn { get; set; }
            public string RefreshToken { get; set; }
            public string TokenType { get; set; }
        }
    }
}
