using Abp.Application.Services;
using Abp.Domain.Repositories;
using Abp.UI;
using EMRSystem.Abdm.Abha.Dto;
using EMRSystem.Abdm.Encryption;
using EMRSystem.Abdm.Sessions;
using EMRSystem.Configuration;
using EMRSystem.Patients;
using Microsoft.Extensions.Options;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace EMRSystem.Abdm.Abha
{
    public class AbhaLoginAppService : ApplicationService, IAbhaLoginAppService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IAbdmSessionManager _sessionManager;
        private readonly IEncryptionService _encryptionService;
        private readonly AbhaConfig _abhaConfig;
        private readonly IRepository<Patient, long> _patientRepository;
        private readonly IRepository<PatientAbhaDetails, long> _patientAbhaRepository;

        public AbhaLoginAppService(
            IHttpClientFactory httpClientFactory,
            IAbdmSessionManager sessionManager,
            IEncryptionService encryptionService,
            IOptions<AbhaConfig> abhaConfigOptions,
            IRepository<Patient, long> patientRepository,
            IRepository<PatientAbhaDetails, long> patientAbhaRepository)
        {
            _httpClientFactory = httpClientFactory;
            _sessionManager = sessionManager;
            _encryptionService = encryptionService;
            _abhaConfig = abhaConfigOptions.Value;
            _patientRepository = patientRepository;
            _patientAbhaRepository = patientAbhaRepository;
        }

        private async Task<HttpClient> CreateConfiguredClientAsync()
        {
            var token = await _sessionManager.GetAccessTokenAsync();
            var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            client.DefaultRequestHeaders.Add("REQUEST-ID", Guid.NewGuid().ToString());
            client.DefaultRequestHeaders.Add("TIMESTAMP", DateTime.UtcNow.ToString("o"));
            return client;
        }

        public async Task<RequestLoginOtpOutput> RequestLoginOtpAsync(RequestLoginOtpInput input)
        {
            var client = await CreateConfiguredClientAsync();
            
            string loginHint = "abha-number";
            string loginIdFormatted = input.LoginId?.Trim() ?? string.Empty;

            if (loginIdFormatted.Contains("@"))
            {
                throw new UserFriendlyException("Logging in via ABHA Address is not directly supported by the ABDM V3 API. Please use your 14-digit ABHA Number or 12-digit Aadhaar.");
            }
            else if (loginIdFormatted.Length == 10 && loginIdFormatted.All(char.IsDigit))
            {
                throw new UserFriendlyException("Mobile Number login is currently disabled as it does not grant permissions to download the ABHA Card or QR Code. Please use your 14-digit ABHA Number or 12-digit Aadhaar.");
            }
            else if (loginIdFormatted.Length == 12 && loginIdFormatted.All(char.IsDigit))
            {
                loginHint = "aadhaar";
            }
            else if (loginIdFormatted.Replace("-", "").Length == 14 && loginIdFormatted.Replace("-", "").All(char.IsDigit))
            {
                loginHint = "abha-number";
                var digits = loginIdFormatted.Replace("-", "");
                loginIdFormatted = $"{digits.Substring(0,2)}-{digits.Substring(2,4)}-{digits.Substring(6,4)}-{digits.Substring(10,4)}";
            }
            else 
            {
                throw new UserFriendlyException("Invalid Login ID format. Please provide a valid 14-digit ABHA Number or 12-digit Aadhaar.");
            }

            string[] scope;
            string otpSystem;

            if (loginHint == "abha-number" || loginHint == "aadhaar")
            {
                scope = new[] { "abha-login", "aadhaar-verify" };
                otpSystem = "aadhaar";
            }
            else
            {
                scope = new[] { "abha-login", "mobile-verify" };
                otpSystem = "abdm";
            }

            var encryptedLoginId = await _encryptionService.EncryptAsync(loginIdFormatted);

            var requestBody = new
            {
                scope = scope,
                loginHint = loginHint,
                loginId = encryptedLoginId,
                otpSystem = otpSystem
            };

            var content = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");
            var response = await client.PostAsync($"{_abhaConfig.BaseUrl.TrimEnd('/')}/v3/profile/login/request/otp", content);

            if (!response.IsSuccessStatusCode)
            {
                var errorResponse = await response.Content.ReadAsStringAsync();
                throw new UserFriendlyException($"ABDM Error: {errorResponse}");
            }

            var responseString = await response.Content.ReadAsStringAsync();
            using var document = JsonDocument.Parse(responseString);
            var txnId = document.RootElement.GetProperty("txnId").GetString();

            return new RequestLoginOtpOutput { TxnId = txnId, Message = "OTP sent successfully" };
        }

        public async Task<VerifyLoginOtpOutput> VerifyLoginOtpAsync(VerifyLoginOtpInput input)
        {
            var client = await CreateConfiguredClientAsync();
            var encryptedOtp = await _encryptionService.EncryptAsync(input.Otp);

            string loginHint = "";
            string loginIdFormatted = input.LoginId ?? "";

            if (loginIdFormatted.Contains("@"))
            {
                throw new UserFriendlyException("Logging in via ABHA Address is not directly supported by the ABDM V3 API. Please use your 14-digit ABHA Number or 12-digit Aadhaar.");
            }
            else if (loginIdFormatted.Length == 10 && loginIdFormatted.All(char.IsDigit))
            {
                throw new UserFriendlyException("Mobile Number login is currently disabled. Please use your 14-digit ABHA Number or 12-digit Aadhaar.");
            }
            else if (loginIdFormatted.Length == 12 && loginIdFormatted.All(char.IsDigit))
            {
                loginHint = "aadhaar";
            }
            else if (loginIdFormatted.Replace("-", "").Length == 14 && loginIdFormatted.Replace("-", "").All(char.IsDigit))
            {
                loginHint = "abha-number";
            }
            else 
            {
                throw new UserFriendlyException("Invalid Login ID format.");
            }

            string[] scope;
            if (loginHint == "abha-number")
            {
                scope = new[] { "abha-login", "aadhaar-verify" };
            }
            else if (loginHint == "aadhaar")
            {
                scope = new[] { "abha-login", "aadhaar-verify" };
            }
            else if (loginHint == "mobile")
            {
                scope = new[] { "abha-login", "mobile-verify" };
            }
            else
            {
                throw new UserFriendlyException("Could not determine scope for verification.");
            }

            var requestBody = new
            {
                scope = scope,
                authData = new
                {
                    authMethods = new[] { "otp" },
                    otp = new
                    {
                        otpValue = encryptedOtp,
                        txnId = input.TxnId
                    }
                }
            };

            var content = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");
            var response = await client.PostAsync($"{_abhaConfig.BaseUrl.TrimEnd('/')}/v3/profile/login/verify", content);

            if (!response.IsSuccessStatusCode)
            {
                var errorResponse = await response.Content.ReadAsStringAsync();
                throw new UserFriendlyException($"ABDM Error: {errorResponse}");
            }

            var responseString = await response.Content.ReadAsStringAsync();
            using var document = JsonDocument.Parse(responseString);

            if (document.RootElement.TryGetProperty("authResult", out var authResultElement) && authResultElement.ValueKind == JsonValueKind.String && authResultElement.GetString() == "failed")
            {
                string message = "OTP verification failed";
                if (document.RootElement.TryGetProperty("message", out var msgElement) && msgElement.ValueKind == JsonValueKind.String)
                {
                    message = msgElement.GetString();
                }
                throw new UserFriendlyException($"ABDM Verify Error: {message}");
            }

            bool isFinalToken = document.RootElement.TryGetProperty("refreshToken", out _);
            if (!isFinalToken && document.RootElement.TryGetProperty("expiresIn", out var expElement))
            {
                if (expElement.ValueKind == JsonValueKind.Number && expElement.GetInt32() > 300) isFinalToken = true;
                else if (expElement.ValueKind == JsonValueKind.String && int.TryParse(expElement.GetString(), out int exp) && exp > 300) isFinalToken = true;
            }

            // If it's an intermediate token, we must call verify/user
            if (!isFinalToken && document.RootElement.TryGetProperty("accounts", out var accountsElement) && accountsElement.ValueKind == JsonValueKind.Array && accountsElement.GetArrayLength() > 0)
            {
                // We got an intermediate token! We must call verify/user to get the final token.
                string abhaNumber = accountsElement[0].GetProperty("ABHANumber").GetString();

                string tToken = string.Empty;
                if (document.RootElement.TryGetProperty("token", out var tTokenElement))
                {
                    tToken = tTokenElement.GetString();
                }

                if (string.IsNullOrWhiteSpace(tToken))
                {
                    throw new UserFriendlyException("ABDM Error: T-token is missing from the initial verify response.");
                }

                string newTxnId = input.TxnId;
                if (document.RootElement.TryGetProperty("txnId", out var txnIdElement))
                {
                    newTxnId = txnIdElement.GetString();
                }

                var userVerifyBody = new
                {
                    ABHANumber = abhaNumber,
                    txnId = newTxnId
                };

                // Construct the request explicitly to guarantee the header is sent correctly
                using var userRequest = new HttpRequestMessage(HttpMethod.Post, $"{_abhaConfig.BaseUrl.TrimEnd('/')}/v3/profile/login/verify/user");
                userRequest.Headers.TryAddWithoutValidation("T-token", $"Bearer {tToken}");
                userRequest.Content = new StringContent(JsonSerializer.Serialize(userVerifyBody), Encoding.UTF8, "application/json");

                // Reuse the existing configured client
                var userResponse = await client.SendAsync(userRequest);

                if (!userResponse.IsSuccessStatusCode)
                {
                    var userError = await userResponse.Content.ReadAsStringAsync();
                    throw new UserFriendlyException($"ABDM Verify User Error: {userError}");
                }

                var userResponseString = await userResponse.Content.ReadAsStringAsync();
                using var userDoc = JsonDocument.Parse(userResponseString);

                if (userDoc.RootElement.TryGetProperty("token", out var finalTokenElement))
                {
                    return new VerifyLoginOtpOutput { XToken = finalTokenElement.GetString(), Message = "OTP Verified & User Selected" };
                }

                throw new UserFriendlyException($"ABDM Error: Final token not found in verify/user response: {userResponseString}");
            }

            // Note: NHA V3 response for login verify returns token at the root level
            string xToken = string.Empty;
            if (document.RootElement.TryGetProperty("token", out var tokenElement))
            {
                xToken = tokenElement.GetString();
            }
            else if (document.RootElement.TryGetProperty("authResult", out var authResultObjElement) && authResultObjElement.ValueKind == JsonValueKind.Object && authResultObjElement.TryGetProperty("X-Token", out var nestedToken))
            {
                xToken = nestedToken.GetString();
            }
            else
            {
                throw new UserFriendlyException($"ABDM Error: Token not found in response. Response: {responseString}");
            }

            return new VerifyLoginOtpOutput { XToken = xToken, Message = "OTP Verified" };
        }

        public async Task<LinkProfileOutput> FetchAndLinkProfileAsync(FetchProfileInput input)
        {
            var client = await CreateConfiguredClientAsync();

            // Explicitly add the X-token via HttpRequestMessage to prevent header dropping
            using var request = new HttpRequestMessage(HttpMethod.Get, $"{_abhaConfig.BaseUrl.TrimEnd('/')}/v3/profile/account");
            request.Headers.TryAddWithoutValidation("X-token", $"Bearer {input.XToken}");

            var response = await client.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                var errorResponse = await response.Content.ReadAsStringAsync();
                throw new UserFriendlyException($"ABDM Error: {errorResponse} | Sent X-Token: {input.XToken}");
            }

            var responseString = await response.Content.ReadAsStringAsync();
            using var document = JsonDocument.Parse(responseString);
            var root = document.RootElement;

            // Corrected to match ABDM V3 spec properties exactly
            var abhaAddress = root.TryGetProperty("preferredAbhaAddress", out var pAddr) ? pAddr.GetString() : string.Empty;
            var abhaNumber = root.TryGetProperty("ABHANumber", out var aNum) ? aNum.GetString() : string.Empty;

            var firstName = root.TryGetProperty("firstName", out var fn) ? fn.GetString() : "";
            var middleName = root.TryGetProperty("middleName", out var ln) ? ln.GetString() : "";
            var name = $"{firstName} {middleName}".Trim();
            var gender = root.GetProperty("gender").GetString();

            int.TryParse(root.GetProperty("yearOfBirth").GetString(), out int yob);
            int.TryParse(root.GetProperty("monthOfBirth").GetString(), out int mob);
            int.TryParse(root.GetProperty("dayOfBirth").GetString(), out int dob);

            if (yob == 0) yob = 1900;
            if (mob == 0) mob = 1;
            if (dob == 0) dob = 1;

            var dateOfBirth = new DateTime(yob, mob, dob);

            var patient = await _patientRepository.GetAsync(input.PatientId);

            patient.FullName = name;
            patient.Gender = gender;
            patient.DateOfBirth = dateOfBirth;

            await _patientRepository.UpdateAsync(patient);

            var existingAbha = await _patientAbhaRepository.FirstOrDefaultAsync(x => x.PatientId == input.PatientId);
            if (existingAbha == null)
            {
                existingAbha = new PatientAbhaDetails
                {
                    PatientId = input.PatientId,
                    EnrolmentReferenceId = "LINKED_VIA_LOGIN"
                };
            }

            existingAbha.AbhaAddress = abhaAddress;
            existingAbha.AbhaNumber = abhaNumber;

            // Fetch and save QR Code and Card
            try
            {
                var qrOutput = await GetAbhaQrCodeAsync(new GetProfileMediaInput { XToken = input.XToken });
                existingAbha.QrCodeBase64 = qrOutput.Base64QrCode;
            }
            catch (Exception ex)
            {
                Logger.Error("Failed to fetch QR Code during profile linking", ex);
            }

            try
            {
                var cardOutput = await GetAbhaCardAsync(new GetProfileMediaInput { XToken = input.XToken });
                existingAbha.CardBase64 = cardOutput.Base64Card;
            }
            catch (Exception ex)
            {
                Logger.Error("Failed to fetch ABHA Card during profile linking", ex);
            }

            if (existingAbha.Id > 0)
            {
                await _patientAbhaRepository.UpdateAsync(existingAbha);
            }
            else
            {
                await _patientAbhaRepository.InsertAsync(existingAbha);
            }

            await CurrentUnitOfWork.SaveChangesAsync();

            return new LinkProfileOutput
            {
                AbhaAddress = abhaAddress,
                AbhaNumber = abhaNumber,
                Name = patient.FullName,
                Message = "Profile linked successfully"
            };
        }

        [HttpPost]
        public async Task<GetAbhaCardOutput> GetAbhaCardAsync(GetProfileMediaInput input)
        {
            var client = await CreateConfiguredClientAsync();

            using var request = new HttpRequestMessage(HttpMethod.Get, $"{_abhaConfig.BaseUrl.TrimEnd('/')}/v3/profile/account/abha-card");

            // Explicitly add the X-Token header
            request.Headers.TryAddWithoutValidation("X-Token", $"Bearer {input.XToken}");

            // Allow default Accept headers to avoid CloudFront WAF blocking

            var response = await client.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                var errorResponse = await response.Content.ReadAsStringAsync();
                var status = response.StatusCode.ToString();
                throw new UserFriendlyException($"ABDM ABHA Card Error: {(string.IsNullOrWhiteSpace(errorResponse) ? status : errorResponse)}");
            }

            var bytes = await response.Content.ReadAsByteArrayAsync();
            return new GetAbhaCardOutput { Base64Card = Convert.ToBase64String(bytes) };
        }

        [HttpPost]
        public async Task<GetAbhaQrCodeOutput> GetAbhaQrCodeAsync(GetProfileMediaInput input)
        {
            var client = await CreateConfiguredClientAsync();

            using var request = new HttpRequestMessage(HttpMethod.Get, $"{_abhaConfig.BaseUrl.TrimEnd('/')}/v3/profile/account/qrCode");

            // Explicitly add the X-Token header
            request.Headers.TryAddWithoutValidation("X-Token", $"Bearer {input.XToken}");

            // Allow default Accept headers to avoid CloudFront WAF blocking

            var response = await client.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                var errorResponse = await response.Content.ReadAsStringAsync();
                var status = response.StatusCode.ToString();
                throw new UserFriendlyException($"ABDM QR Code Error: {(string.IsNullOrWhiteSpace(errorResponse) ? status : errorResponse)}");
            }

            var bytes = await response.Content.ReadAsByteArrayAsync();
            return new GetAbhaQrCodeOutput { Base64QrCode = Convert.ToBase64String(bytes) };
        }
    }
}
