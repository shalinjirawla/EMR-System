using Abp.Dependency;
using EMRSystem.Abdm.Sessions;
using EMRSystem.Configuration;
using Microsoft.Extensions.Options;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace EMRSystem.Abdm.Encryption
{
    public class EncryptionService : IEncryptionService, ITransientDependency
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IAbdmSessionManager _sessionManager;
        private readonly AbhaConfig _abhaConfig;

        private static string _cachedPublicKey;

        public EncryptionService(
            IHttpClientFactory httpClientFactory,
            IAbdmSessionManager sessionManager,
            IOptions<AbhaConfig> abhaConfigOptions)
        {
            _httpClientFactory = httpClientFactory;
            _sessionManager = sessionManager;
            _abhaConfig = abhaConfigOptions.Value;
        }

        private async Task<string> GetPublicKeyAsync()
        {
            if (!string.IsNullOrEmpty(_cachedPublicKey))
            {
                return _cachedPublicKey;
            }

            var token = await _sessionManager.GetAccessTokenAsync();
            var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            client.DefaultRequestHeaders.Add("REQUEST-ID", Guid.NewGuid().ToString());
            client.DefaultRequestHeaders.Add("TIMESTAMP", DateTime.UtcNow.ToString("o"));

            var response = await client.GetAsync($"{_abhaConfig.BaseUrl.TrimEnd('/')}/v3/profile/public/certificate");
            response.EnsureSuccessStatusCode();

            var responseString = await response.Content.ReadAsStringAsync();
            try
            {
                using var doc = JsonDocument.Parse(responseString);
                if (doc.RootElement.TryGetProperty("publicKey", out var pkElement))
                {
                    _cachedPublicKey = pkElement.GetString();
                }
                else
                {
                    _cachedPublicKey = responseString;
                }
            }
            catch
            {
                _cachedPublicKey = responseString;
            }

            return _cachedPublicKey;
        }

        public async Task<string> EncryptAsync(string plainText)
        {
            if (string.IsNullOrEmpty(plainText))
                throw new ArgumentNullException(nameof(plainText));

            var publicKeyPem = await GetPublicKeyAsync();

            using var rsa = RSA.Create();
            try
            {
                rsa.ImportFromPem(publicKeyPem);
            }
            catch (ArgumentException)
            {
                // In case the gateway returns a raw base64 instead of PEM headers
                // Or if we need to clean up quotes from a JSON string
                publicKeyPem = publicKeyPem.Trim('"');
                if (!publicKeyPem.Contains("-----BEGIN"))
                {
                    publicKeyPem = $"-----BEGIN PUBLIC KEY-----\n{publicKeyPem}\n-----END PUBLIC KEY-----";
                }
                rsa.ImportFromPem(publicKeyPem);
            }

            var plainTextBytes = Encoding.UTF8.GetBytes(plainText);
            var encryptedBytes = rsa.Encrypt(plainTextBytes, RSAEncryptionPadding.OaepSHA1);

            return Convert.ToBase64String(encryptedBytes);
        }
    }
}
