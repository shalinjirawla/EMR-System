using Abp.Application.Services;
using Abp.Domain.Repositories;
using Abp.Runtime.Caching;
using Abp.UI;
using EMRSystem.Abdm.Authentication;
using EMRSystem.Abdm.Encryption;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace EMRSystem.Abdm.HealthInformation
{
    public interface IAbdmHealthInformationAppService : IApplicationService
    {
        Task<string> RequestExternalHealthInformationAsync(string consentId, string dateFrom = null, string dateTo = null);
        Task<List<AbdmExternalHealthRecordDto>> GetExternalHealthRecordsAsync(int patientId);
    }

    public class AbdmExternalHealthRecordDto
    {
        public string TransactionId { get; set; }
        public string FhirPayload { get; set; }
        public string ExtractedSummary { get; set; }
        public DateTime CreationTime { get; set; }
    }

    public class AbdmHealthInformationAppService : ApplicationService, IAbdmHealthInformationAppService
    {
        private readonly IAbdmTokenService _tokenService;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IAbdmEncryptionService _encryptionService;
        private readonly ICacheManager _cacheManager;
        private readonly IRepository<AbdmExternalHealthRecord, long> _externalRecordRepository;

        public AbdmHealthInformationAppService(
            IAbdmTokenService tokenService,
            IHttpClientFactory httpClientFactory,
            IAbdmEncryptionService encryptionService,
            ICacheManager cacheManager,
            IRepository<AbdmExternalHealthRecord, long> externalRecordRepository)
        {
            _tokenService = tokenService;
            _httpClientFactory = httpClientFactory;
            _encryptionService = encryptionService;
            _cacheManager = cacheManager;
            _externalRecordRepository = externalRecordRepository;
        }

        public async Task<List<AbdmExternalHealthRecordDto>> GetExternalHealthRecordsAsync(int patientId)
        {
            var records = await _externalRecordRepository.GetAllListAsync(x => x.PatientId == patientId);
            var dtos = new List<AbdmExternalHealthRecordDto>();
            foreach (var record in records)
            {
                dtos.Add(new AbdmExternalHealthRecordDto
                {
                    TransactionId = record.TransactionId,
                    FhirPayload = record.FhirPayload,
                    ExtractedSummary = record.ExtractedSummary,
                    CreationTime = record.CreationTime
                });
            }
            return dtos;
        }

        public async Task<string> RequestExternalHealthInformationAsync(string consentId, string dateFrom = null, string dateTo = null)
        {
            // 1. Generate DH KeyPair
            var keyPair = await _encryptionService.GenerateKeyPairAsync();
            var nonceBytes = new byte[32];
            new Random().NextBytes(nonceBytes);
            var nonceBase64 = Convert.ToBase64String(nonceBytes);

            // 2. Store Private Key securely (using Cache for simplicity)
            var cache = _cacheManager.GetCache("AbdmKeys");
            
            // 3. Make API call to Gateway
            var token = await _tokenService.GetAccessTokenAsync();
            var client = _httpClientFactory.CreateClient("AbdmGateway");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            client.DefaultRequestHeaders.Add("X-HIU-ID", "IN2410002555");
            client.DefaultRequestHeaders.Add("X-CM-ID", "sbx");

            var requestId = Guid.NewGuid().ToString();
            
            await cache.SetAsync(requestId, keyPair.PrivateKeyBase64, TimeSpan.FromDays(1));
            
            var timestamp = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffZ");
            client.DefaultRequestHeaders.Add("REQUEST-ID", requestId);
            client.DefaultRequestHeaders.Add("TIMESTAMP", timestamp);

            // The exact webhook endpoint for data push. They must update the ngrok domain if it changes.
            // But usually this config comes from appsettings.json
            var dataPushUrl = "https://monkfish-pleasant-gnat.ngrok-free.app/api/abdm/hiu/data-push";

            var payload = new
            {
                requestId = requestId,
                timestamp = timestamp,
                hiRequest = new
                {
                    consent = new { id = consentId },
                    dateRange = new
                    {
                        from = dateFrom ?? DateTime.UtcNow.AddYears(-5).ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
                        to = dateTo ?? DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffZ")
                    },
                    dataPushUrl = dataPushUrl,
                    keyMaterial = new
                    {
                        cryptoAlg = "ECDH",
                        curve = "curve25519",
                        dhPublicKey = new
                        {
                            expiry = DateTime.UtcNow.AddDays(1).ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
                            parameters = "Curve25519/32byte random key",
                            keyValue = keyPair.PublicKeyBase64
                        },
                        nonce = nonceBase64
                    }
                }
            };

            var content = new StringContent(JsonConvert.SerializeObject(payload), Encoding.UTF8, "application/json");
            var response = await client.PostAsync("https://dev.abdm.gov.in/api/hiecm/data-flow/v3/health-information/request", content);

            if (!response.IsSuccessStatusCode)
            {
                var errorResponse = await response.Content.ReadAsStringAsync();
                throw new UserFriendlyException($"HI Request Failed: {errorResponse}");
            }

            return requestId;
        }
    }
}
