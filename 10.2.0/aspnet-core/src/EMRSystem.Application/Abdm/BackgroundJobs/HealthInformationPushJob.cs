using Abp.BackgroundJobs;
using Microsoft.EntityFrameworkCore;
using Abp.Dependency;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using EMRSystem.Abdm.Authentication;
using EMRSystem.Abdm.Encryption;
using EMRSystem.Abdm.FHIR;
using EMRSystem.Abdm.HealthInformation;
using EMRSystem.Prescriptions;
using Hl7.Fhir.Serialization;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace EMRSystem.Abdm.BackgroundJobs
{
    [Serializable]
    public class HealthInformationPushJobArgs
    {
        public long TaskId { get; set; }
        public string TransactionId { get; set; }
        public string ConsentId { get; set; }
        public string DataPushUrl { get; set; }
        public string RemotePublicKeyBase64 { get; set; }
        public string RemoteNonceBase64 { get; set; }
    }

    public class HealthInformationPushJob : AsyncBackgroundJob<HealthInformationPushJobArgs>, ITransientDependency
    {
        private readonly IRepository<AbdmHealthInformationTask, long> _hiTaskRepository;
        private readonly IRepository<Prescription, long> _prescriptionRepository;
        private readonly IFhirBundleGenerator _fhirGenerator;
        private readonly IAbdmEncryptionService _encryptionService;
        private readonly IAbdmTokenService _tokenService;
        private readonly IHttpClientFactory _httpClientFactory;

        public HealthInformationPushJob(
            IRepository<AbdmHealthInformationTask, long> hiTaskRepository,
            IRepository<Prescription, long> prescriptionRepository,
            IFhirBundleGenerator fhirGenerator,
            IAbdmEncryptionService encryptionService,
            IAbdmTokenService tokenService,
            IHttpClientFactory httpClientFactory)
        {
            _hiTaskRepository = hiTaskRepository;
            _prescriptionRepository = prescriptionRepository;
            _fhirGenerator = fhirGenerator;
            _encryptionService = encryptionService;
            _tokenService = tokenService;
            _httpClientFactory = httpClientFactory;
        }

        [UnitOfWork]
        public override async Task ExecuteAsync(HealthInformationPushJobArgs args)
        {
            var taskRecord = await _hiTaskRepository.GetAsync(args.TaskId);
            try
            {
                taskRecord.Status = "PROCESSING";
                await _hiTaskRepository.UpdateAsync(taskRecord);

                // 1. Fetch relevant data (Simulated finding a prescription by consent rules)
                var prescription = await _prescriptionRepository.GetAll().FirstOrDefaultAsync();
                if (prescription == null)
                {
                    Logger.Warn("No prescriptions found in the database. Cannot push health information.");
                    taskRecord.Status = "FAILED_NO_DATA";
                    await _hiTaskRepository.UpdateAsync(taskRecord);
                    return;
                }
                
                // 2. Generate FHIR Bundle
                var bundle = await _fhirGenerator.GeneratePrescriptionBundleAsync(prescription.Id);
                var fhirJson = new FhirJsonSerializer().SerializeToString(bundle);

                // 3. Generate Local DH KeyPair
                var keyPair = await _encryptionService.GenerateKeyPairAsync();

                // 4. Encrypt FHIR Data
                var encryptedData = await _encryptionService.EncryptAsync(fhirJson, args.RemotePublicKeyBase64, keyPair.PrivateKeyBase64);

                // 5. Build Push Payload
                var payload = new
                {
                    pageNumber = 1,
                    pageCount = 1,
                    transactionId = args.TransactionId,
                    entries = new[]
                    {
                        new
                        {
                            content = encryptedData.CipherTextBase64,
                            media = "application/fhir+json",
                            checksum = "MD5_CHECKSUM_HERE",
                            careContextReference = "CareContext-1"
                        }
                    },
                    keyMaterial = new
                    {
                        cryptoAlg = "ECDH",
                        curve = "Curve25519",
                        dhPublicKey = new
                        {
                            expiry = DateTime.UtcNow.AddDays(1).ToString("o"),
                            parameters = "Curve25519",
                            keyValue = keyPair.PublicKeyBase64
                        },
                        nonce = encryptedData.NonceBase64
                    }
                };

                // 6. Push to ABDM HIU Data Push URL
                var client = _httpClientFactory.CreateClient();
                var token = await _tokenService.GetAccessTokenAsync();
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                
                var content = new StringContent(JsonConvert.SerializeObject(payload), Encoding.UTF8, "application/json");
                var response = await client.PostAsync(args.DataPushUrl, content);

                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception($"Data Push Failed: {await response.Content.ReadAsStringAsync()}");
                }

                taskRecord.Status = "COMPLETED";
            }
            catch (Exception ex)
            {
                taskRecord.Status = "FAILED";
                taskRecord.ErrorMessage = ex.Message;
                Logger.Error($"HIP Data Push Failed for Transaction {args.TransactionId}", ex);
            }
            finally
            {
                await _hiTaskRepository.UpdateAsync(taskRecord);
            }
        }
    }
}
