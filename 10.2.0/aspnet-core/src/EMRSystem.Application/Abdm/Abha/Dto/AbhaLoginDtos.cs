namespace EMRSystem.Abdm.Abha.Dto
{
    public class RequestLoginOtpInput
    {
        public string LoginId { get; set; }
    }
    
    public class RequestLoginOtpOutput
    {
        public string TxnId { get; set; }
        public string Message { get; set; }
    }
    
    public class VerifyLoginOtpInput
    {
        public string Otp { get; set; }
        public string TxnId { get; set; }
        public string LoginId { get; set; }
    }
    
    public class VerifyLoginOtpOutput
    {
        public string XToken { get; set; }
        public string Message { get; set; }
    }
    
    public class FetchProfileInput
    {
        public long PatientId { get; set; }
        public string XToken { get; set; }
    }
    
    public class LinkProfileOutput
    {
        public string AbhaNumber { get; set; }
        public string AbhaAddress { get; set; }
        public string Name { get; set; }
        public string Message { get; set; }
    }

    public class GetProfileMediaInput
    {
        public string XToken { get; set; }
    }
    
    public class GetAbhaCardOutput
    {
        public string Base64Card { get; set; }
    }
    
    public class GetAbhaQrCodeOutput
    {
        public string Base64QrCode { get; set; }
    }
}
