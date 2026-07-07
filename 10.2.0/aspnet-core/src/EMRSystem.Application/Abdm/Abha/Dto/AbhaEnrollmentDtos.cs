using System.Collections.Generic;

namespace EMRSystem.Abdm.Abha.Dto
{
    public class RequestAadhaarOtpInput
    {
        public string AadhaarNumber { get; set; }
    }

    public class RequestAadhaarOtpOutput
    {
        public string TxnId { get; set; }
        public string Message { get; set; }
    }

    public class VerifyAadhaarOtpInput
    {
        public string Otp { get; set; }
        public string TxnId { get; set; }
        public string MobileNumber { get; set; }
    }

    public class VerifyAadhaarOtpOutput
    {
        public string TxnId { get; set; }
        public AbhaProfileDto Profile { get; set; }
        public string Message { get; set; }
    }

    public class AbhaProfileDto
    {
        public string Name { get; set; }
        public string Gender { get; set; }
        public string YearOfBirth { get; set; }
        public string Photo { get; set; }
        public List<string> PhrAddress { get; set; } = new List<string>();
        public string AbhaNumber { get; set; }
    }

    public class SuggestAddressInput
    {
        public string TxnId { get; set; }
    }

    public class CreateAbhaAddressInput
    {
        public long PatientId { get; set; }
        public string AbhaAddress { get; set; }
        public string TxnId { get; set; }
    }

    public class CreateAbhaAddressOutput
    {
        public string AbhaNumber { get; set; }
        public string AbhaAddress { get; set; }
        public string EnrolmentReferenceId { get; set; }
        public string Message { get; set; }
    }
}
