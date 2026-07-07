using Abp.Application.Services;
using Abp.Domain.Repositories;
using Abp.UI;
using EMRSystem.Abdm.Abha;
using EMRSystem.Abdm.Authentication;
using EMRSystem.Patients;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Abp.Runtime.Caching;
using System.Threading.Tasks;

namespace EMRSystem.Abdm.Consent
{
    public class InitiateConsentRequestDto
    {
        public long PatientId { get; set; }
        public string DoctorName { get; set; }
        public string PurposeCode { get; set; } = "CAREMGT";
    }

    public interface IAbdmConsentAppService : IApplicationService
    {
        Task<string> InitiateConsentRequestAsync(InitiateConsentRequestDto input);
    }

    public class AbdmConsentAppService : ApplicationService, IAbdmConsentAppService
    {
        private readonly IRepository<Patient, long> _patientRepository;
        private readonly IAbdmTokenService _tokenService;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IRepository<PatientAbhaDetails, long> _patientAbhaRepository;
        private readonly ICacheManager _cacheManager;

        public AbdmConsentAppService(
            IRepository<Patient, long> patientRepository,
            IAbdmTokenService tokenService,
            IHttpClientFactory httpClientFactory,
            IRepository<PatientAbhaDetails, long> patientAbhaRepository,
            ICacheManager cacheManager)
        {
            _patientRepository = patientRepository;
            _tokenService = tokenService;
            _httpClientFactory = httpClientFactory;
            _patientAbhaRepository = patientAbhaRepository;
            _cacheManager = cacheManager;
        }

        public async Task<string> InitiateConsentRequestAsync(InitiateConsentRequestDto input)
        {
            var dateFrom = DateTime.UtcNow.AddYears(-5).ToString("yyyy-MM-ddTHH:mm:ss.fffZ");
            var dateTo = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffZ");

            var metaJson = JsonConvert.SerializeObject(new { 
                TenantId = AbpSession.TenantId, 
                PatientId = input.PatientId,
                DateFrom = dateFrom,
                DateTo = dateTo
            });

            var abhaDetails = await _patientAbhaRepository.FirstOrDefaultAsync(x => x.PatientId == input.PatientId);
            if (abhaDetails == null || string.IsNullOrEmpty(abhaDetails.AbhaAddress))
            {
                throw new UserFriendlyException("Patient does not have a linked ABHA Address.");
            }

            var token = await _tokenService.GetAccessTokenAsync();
            var client = _httpClientFactory.CreateClient("AbdmGateway");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            client.DefaultRequestHeaders.Add("X-HIU-ID", "IN2410002555"); 

            var requestId = Guid.NewGuid().ToString();
            var timestamp = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffZ");

            client.DefaultRequestHeaders.Add("X-CM-ID", "sbx");
            client.DefaultRequestHeaders.Add("REQUEST-ID", requestId);
            client.DefaultRequestHeaders.Add("TIMESTAMP", timestamp);

            var payload = new
            {
                requestId = requestId,
                timestamp = timestamp,
                consent = new
                {
                    purpose = new { text = "Care Management", code = input.PurposeCode, refUri = "http://terminology.hl7.org/ValueSet/v3-PurposeOfUse" },
                    patient = new { id = abhaDetails.AbhaAddress },
                    hiu = new { id = "IN2410002555" },
                    requester = new { name = input.DoctorName, identifier = new { type = "REGNO", value = "DOC123", system = "https://hpr.ndhm.gov.in" } },
                    hiTypes = new[] { "Prescription", "DiagnosticReport", "DischargeSummary" },
                    permission = new
                    {
                        accessMode = "VIEW",
                        dateRange = new
                        {
                            from = dateFrom,
                            to = dateTo
                        },
                        dataEraseAt = DateTime.UtcNow.AddDays(30).ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
                        frequency = new { unit = "HOUR", value = 1, repeats = 0 }
                    }
                }
            };

            var content = new StringContent(JsonConvert.SerializeObject(payload), Encoding.UTF8, "application/json");
            var response = await client.PostAsync("https://dev.abdm.gov.in/api/hiecm/consent/v3/request/init", content);

            if (!response.IsSuccessStatusCode)
            {
                throw new UserFriendlyException($"Consent Initiation Failed: {await response.Content.ReadAsStringAsync()}");
            }

            var cache = _cacheManager.GetCache<string, string>("AbdmConsentRequests");
            cache.Set(requestId, metaJson, TimeSpan.FromDays(7));

            return requestId; // The status will be updated via Webhooks
        }
    }
}
