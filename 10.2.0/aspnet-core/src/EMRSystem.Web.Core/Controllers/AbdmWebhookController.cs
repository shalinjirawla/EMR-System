using Abp.Domain.Repositories;
using EMRSystem.Abdm.Consent;
using EMRSystem.Abdm.HealthInformation;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Text.Json;
using System.Threading.Tasks;
using Abp.BackgroundJobs;
using EMRSystem.Abdm.BackgroundJobs;
using EMRSystem.Abdm.Encryption;
using Abp.Runtime.Caching;

namespace EMRSystem.Controllers
{
    [Route("api/abdm")]
    [IgnoreAntiforgeryToken]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class AbdmWebhookController : EMRSystemControllerBase
    {
        private readonly IRepository<AbdmConsentRequest, long> _consentRepository;
        private readonly IRepository<AbdmHealthInformationTask, long> _hiTaskRepository;
        private readonly IBackgroundJobManager _backgroundJobManager;
        private readonly IAbdmEncryptionService _encryptionService;
        private readonly ICacheManager _cacheManager;
        private readonly IRepository<AbdmExternalHealthRecord, long> _externalRecordRepository;
        private readonly IAbdmHealthInformationAppService _healthInfoAppService;

        public AbdmWebhookController(
            IRepository<AbdmConsentRequest, long> consentRepository,
            IRepository<AbdmHealthInformationTask, long> hiTaskRepository,
            IBackgroundJobManager backgroundJobManager,
            IAbdmEncryptionService encryptionService,
            ICacheManager cacheManager,
            IRepository<AbdmExternalHealthRecord, long> externalRecordRepository,
            IAbdmHealthInformationAppService healthInfoAppService)
        {
            _consentRepository = consentRepository;
            _hiTaskRepository = hiTaskRepository;
            _backgroundJobManager = backgroundJobManager;
            _encryptionService = encryptionService;
            _cacheManager = cacheManager;
            _externalRecordRepository = externalRecordRepository;
            _healthInfoAppService = healthInfoAppService;
        }

        [HttpPost("v3/consent/request/hip/notify")]
        [HttpPost("api/v3/consent/request/hip/notify")]

        public async Task<IActionResult> ConsentNotify()
        {
            string body;
            using (var reader = new System.IO.StreamReader(Request.Body))
            {
                body = await reader.ReadToEndAsync();
            }
            var payload = JsonDocument.Parse(body).RootElement;
            // The ABDM Gateway sends consent notification.
            // Acknowledge receipt first, then process.
            
            string requestId = payload.TryGetProperty("requestId", out var reqProp) && reqProp.ValueKind != System.Text.Json.JsonValueKind.Null ? reqProp.GetString() : null;
            if (string.IsNullOrEmpty(requestId))
            {
                string headerVal = Request.Headers["REQUEST-ID"].ToString();
                requestId = string.IsNullOrEmpty(headerVal) ? Guid.NewGuid().ToString() : headerVal;
            }
            var notification = payload.GetProperty("notification");
            string status = notification.TryGetProperty("status", out var statusProp) ? statusProp.GetString() : null;
            string consentId = notification.TryGetProperty("consentId", out var consentIdProp) ? consentIdProp.GetString() : 
                               (notification.TryGetProperty("consentRequestId", out var consentReqIdProp) ? consentReqIdProp.GetString() : null);

            if (status == "GRANTED" || status == "REVOKED")
            {
                using (CurrentUnitOfWork.DisableFilter(Abp.Domain.Uow.AbpDataFilters.MustHaveTenant))
                {
                    var consentRequest = await _consentRepository.FirstOrDefaultAsync(c => c.ConsentId == consentId);
                    if (consentRequest != null)
                    {
                        using (CurrentUnitOfWork.SetTenantId(consentRequest.TenantId))
                        {
                            consentRequest.Status = status;
                            await _consentRepository.UpdateAsync(consentRequest);
                        }
                    }
                    else
                    {
                        Logger.Error($"HIP Consent Notify: Consent ID {consentId} not found in database. Will still send on-notify to Gateway.");
                    }
                }

                // Call Gateway /consent/hip/on-notify to acknowledge receipt
                var ackPayload = new
                {
                    requestId = Guid.NewGuid().ToString(),
                    timestamp = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
                    acknowledgement = new
                    {
                        status = "OK",
                        consentId = consentId
                    },
                    response = new
                    {
                        requestId = requestId
                    },
                    resp = new
                    {
                        requestId = requestId
                    }
                };
                
                var client = new System.Net.Http.HttpClient();
                var tokenService = HttpContext.RequestServices.GetService(typeof(EMRSystem.Abdm.Authentication.IAbdmTokenService)) as EMRSystem.Abdm.Authentication.IAbdmTokenService;
                var token = await tokenService.GetAccessTokenAsync();
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                client.DefaultRequestHeaders.Add("X-CM-ID", "sbx");
                client.DefaultRequestHeaders.Add("REQUEST-ID", Guid.NewGuid().ToString());
                client.DefaultRequestHeaders.Add("TIMESTAMP", DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"));
                var ackContent = new System.Net.Http.StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(ackPayload), System.Text.Encoding.UTF8, "application/json");
                var ackResp = await client.PostAsync("https://dev.abdm.gov.in/api/hiecm/consent/v3/request/hip/on-notify", ackContent);
                if (!ackResp.IsSuccessStatusCode)
                {
                    Logger.Error($"HIP on-notify failed with status {ackResp.StatusCode}: {await ackResp.Content.ReadAsStringAsync()}");
                }
                else
                {
                    Logger.Info($"HIP on-notify sent successfully for Consent ID: {consentId}");
                }
            }

            return Accepted();
        }

        [HttpPost("v3/hiu/consent/request/on-init")]
        [HttpPost("api/v3/hiu/consent/request/on-init")]
        public async Task<IActionResult> OnConsentInit()
        {
            string body;
            using (var reader = new System.IO.StreamReader(Request.Body))
            {
                body = await reader.ReadToEndAsync();
            }
            var payload = JsonDocument.Parse(body).RootElement;
            try
            {
                var response = payload.GetProperty("response");
                var reqId = response.GetProperty("requestId").GetString(); // Our original requestId

                if (payload.TryGetProperty("consentRequest", out var consentRequest))
                {
                    var consentReqId = consentRequest.GetProperty("id").GetString();
                    
                    var cache = _cacheManager.GetCache<string, string>("AbdmConsentRequests");
                    var metaJson = cache.GetOrDefault(reqId);
                    if (metaJson != null)
                    {
                        cache.Set(consentReqId, metaJson, TimeSpan.FromDays(7));
                        Logger.Info($"Mapped RequestId {reqId} to ConsentRequestId {consentReqId}");
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error("Error processing on-init webhook", ex);
            }
            return Accepted();
        }

        [HttpPost("v3/hiu/consent/request/notify")]
        [HttpPost("api/v3/hiu/consent/request/notify")]
        public async Task<IActionResult> HiuConsentNotify()
        {
            string body;
            using (var reader = new System.IO.StreamReader(Request.Body))
            {
                body = await reader.ReadToEndAsync();
            }
            var payload = JsonDocument.Parse(body).RootElement;
            try
            {
                var notification = payload.GetProperty("notification");
                var status = notification.GetProperty("status").GetString();

                var consentReqId = notification.TryGetProperty("consentRequestId", out var crIdProp) ? crIdProp.GetString() : null;
                int tenantId = 1;
                long patientId = 1;
                string dateFrom = null;
                string dateTo = null;
                if (consentReqId != null)
                {
                    var cache = _cacheManager.GetCache<string, string>("AbdmConsentRequests");
                    var metaJson = cache.GetOrDefault(consentReqId);
                    if (metaJson != null)
                    {
                        var doc = JsonDocument.Parse(metaJson).RootElement;
                        tenantId = doc.GetProperty("TenantId").GetInt32();
                        patientId = doc.GetProperty("PatientId").GetInt64();
                        
                        if (doc.TryGetProperty("DateFrom", out var dfProp)) dateFrom = dfProp.GetString();
                        if (doc.TryGetProperty("DateTo", out var dtProp)) dateTo = dtProp.GetString();
                    }
                }

                using (CurrentUnitOfWork.SetTenantId(tenantId))
                {
                    if (status == "GRANTED")
                    {
                        var consentArtefacts = notification.GetProperty("consentArtefacts");
                        foreach (var artefact in consentArtefacts.EnumerateArray())
                        {
                            var consentId = artefact.GetProperty("id").GetString();
                            
                            var existingConsent = await _consentRepository.FirstOrDefaultAsync(c => c.ConsentId == consentId);
                            if (existingConsent == null)
                            {
                                await _consentRepository.InsertAsync(new AbdmConsentRequest {
                                    TenantId = tenantId,
                                    ConsentId = consentId,
                                    PatientId = patientId,
                                    Status = status,
                                    Purpose = "Care Management"
                                });
                                await CurrentUnitOfWork.SaveChangesAsync();
                            }

                            Logger.Info($"Consent GRANTED (HIU). Automatically requesting health information for Consent ID: {consentId}");
                            
                            // Automatically trigger data request!
                            await Task.Delay(5000); // Wait 5 seconds for gateway to register HIP's on-notify to prevent race conditions
                            await _healthInfoAppService.RequestExternalHealthInformationAsync(consentId, dateFrom, dateTo);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error("Error processing HIU Consent Notification", ex);
            }
            return Accepted();
        }

        [HttpPost("v3/hip/health-information/request")]
        [HttpPost("api/v3/hip/health-information/request")]
        public async Task<IActionResult> HealthInformationRequest()
        {
            string body;
            using (var reader = new System.IO.StreamReader(Request.Body))
            {
                body = await reader.ReadToEndAsync();
            }
            var payload = JsonDocument.Parse(body).RootElement;
            var hiRequest = payload.GetProperty("hiRequest");
            var transactionId = payload.TryGetProperty("transactionId", out var tIdProp) ? tIdProp.GetString() : hiRequest.GetProperty("transactionId").GetString();
            var consentId = hiRequest.GetProperty("consent").GetProperty("id").GetString();
            var dataPushUrl = hiRequest.GetProperty("dataPushUrl").GetString();
            var keyMaterial = hiRequest.GetProperty("keyMaterial");

            var remotePublicKey = keyMaterial.GetProperty("dhPublicKey").GetProperty("keyValue").GetString();
            var remoteNonce = keyMaterial.GetProperty("nonce").GetString();

            int tenantId = 1;
            using (CurrentUnitOfWork.DisableFilter(Abp.Domain.Uow.AbpDataFilters.MustHaveTenant))
            {
                var consentRequest = await _consentRepository.FirstOrDefaultAsync(c => c.ConsentId == consentId);
                if (consentRequest != null)
                {
                    tenantId = consentRequest.TenantId;
                }
            }

            using (CurrentUnitOfWork.SetTenantId(tenantId))
            {
                var task = new AbdmHealthInformationTask
                {
                    TenantId = tenantId,
                    TransactionId = transactionId,
                    ConsentId = consentId,
                    Status = "PENDING"
                };
                await _hiTaskRepository.InsertAsync(task);
                await CurrentUnitOfWork.SaveChangesAsync(); // Ensure ID is generated

                // Queue background job to process the data push securely
                _backgroundJobManager.Enqueue<HealthInformationPushJob, HealthInformationPushJobArgs>(
                    new HealthInformationPushJobArgs
                    {
                        TaskId = task.Id,
                        TransactionId = transactionId,
                        ConsentId = consentId,
                        DataPushUrl = dataPushUrl,
                        RemotePublicKeyBase64 = remotePublicKey,
                        RemoteNonceBase64 = remoteNonce
                    }
                );
            }

            return Accepted();
        }

        [HttpPost("hiu/data-push")]
        public async Task<IActionResult> HiuDataPush()
        {
            string body;
            using (var reader = new System.IO.StreamReader(Request.Body))
            {
                body = await reader.ReadToEndAsync();
            }
            var payload = JsonDocument.Parse(body).RootElement;
            try
            {
                var entries = payload.GetProperty("entries");
                var keyMaterial = payload.GetProperty("keyMaterial");
                var transactionId = payload.GetProperty("transactionId").GetString();

                // Extract remote public key & nonce
                var remotePublicKeyBase64 = keyMaterial.GetProperty("dhPublicKey").GetProperty("keyValue").GetString();
                var remoteNonceBase64 = keyMaterial.GetProperty("nonce").GetString();

                var cache = _cacheManager.GetCache<string, string>("AbdmKeys");
                var localPrivateKeyBase64 = cache.GetOrDefault(transactionId);

                if (string.IsNullOrEmpty(localPrivateKeyBase64))
                {
                    Logger.Error($"Private key not found for TransactionId: {transactionId}");
                    return Accepted();
                }

                int tenantId = 1;
                long patientId = 1;
                using (CurrentUnitOfWork.DisableFilter(Abp.Domain.Uow.AbpDataFilters.MustHaveTenant))
                {
                    var hiTask = await _hiTaskRepository.FirstOrDefaultAsync(t => t.TransactionId == transactionId);
                    if (hiTask != null)
                    {
                        tenantId = hiTask.TenantId;
                        var consentReq = await _consentRepository.FirstOrDefaultAsync(c => c.ConsentId == hiTask.ConsentId);
                        if (consentReq != null) patientId = consentReq.PatientId;
                    }
                }

                using (CurrentUnitOfWork.SetTenantId(tenantId))
                {
                    foreach (var entry in entries.EnumerateArray())
                    {
                        if (entry.TryGetProperty("content", out var contentElement) && contentElement.ValueKind == JsonValueKind.String)
                        {
                            var cipherText = contentElement.GetString();
                            var decryptedJson = await _encryptionService.DecryptAsync(cipherText, remoteNonceBase64, remotePublicKeyBase64, localPrivateKeyBase64);

                            // Save to Db
                            var record = new AbdmExternalHealthRecord
                            {
                                TenantId = tenantId,
                                TransactionId = transactionId,
                                FhirPayload = decryptedJson,
                                ExtractedSummary = "Decrypted Health Information",
                                PatientId = (int)patientId
                            };
                            await _externalRecordRepository.InsertAsync(record);
                            Logger.Info("Successfully decrypted and saved health record.");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error("Error decrypting data push", ex);
            }

            return Accepted();
        }

        [HttpPost("v3/hiu/health-information/on-request")]
        [HttpPost("api/v3/hiu/health-information/on-request")]
        public async Task<IActionResult> OnRequest()
        {
            string body;
            using (var reader = new System.IO.StreamReader(Request.Body))
            {
                body = await reader.ReadToEndAsync();
            }
            var payload = JsonDocument.Parse(body).RootElement;
            try
            {
                // Check if gateway returned an error
                if (payload.TryGetProperty("error", out var errorElement) && errorElement.ValueKind != JsonValueKind.Null)
                {
                    Logger.Error($"ABDM Gateway returned error in on-request: {errorElement.ToString()}");
                    return Accepted();
                }

                if (payload.TryGetProperty("hiRequest", out var hiRequest) && hiRequest.ValueKind != JsonValueKind.Null)
                {
                    var transactionId = hiRequest.GetProperty("transactionId").GetString();
                    
                    // Gateway v3 usually sends 'resp', but check both just in case
                    string reqId = null;
                    if (payload.TryGetProperty("resp", out var resp) && resp.ValueKind != JsonValueKind.Null)
                    {
                        reqId = resp.GetProperty("requestId").GetString();
                    }
                    else if (payload.TryGetProperty("response", out var response) && response.ValueKind != JsonValueKind.Null)
                    {
                        reqId = response.GetProperty("requestId").GetString();
                    }

                    if (!string.IsNullOrEmpty(reqId))
                    {
                        var cache = _cacheManager.GetCache<string, string>("AbdmKeys");
                        var privateKey = cache.GetOrDefault(reqId);
                        if (!string.IsNullOrEmpty(privateKey))
                        {
                            cache.Set(transactionId, privateKey, TimeSpan.FromDays(1));
                            Logger.Info($"Successfully mapped RequestId {reqId} to TransactionId {transactionId} in cache!");
                        }
                        else
                        {
                            Logger.Warn($"Private key not found in cache for RequestId {reqId}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error("Error processing on-request webhook", ex);
            }
            return Accepted();
        }
    }
}
