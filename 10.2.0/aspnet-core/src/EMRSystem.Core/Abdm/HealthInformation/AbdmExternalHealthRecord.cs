using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;

namespace EMRSystem.Abdm.HealthInformation
{
    public class AbdmExternalHealthRecord : FullAuditedEntity<long>, IMustHaveTenant
    {
        public int TenantId { get; set; }
        public int PatientId { get; set; }
        public string ConsentId { get; set; }
        public string TransactionId { get; set; }
        public string FhirPayload { get; set; }
        public string ExtractedSummary { get; set; }
    }
}
