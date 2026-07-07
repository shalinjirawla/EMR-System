using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using EMRSystem.Patients;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace EMRSystem.Abdm.Consent
{
    public class AbdmConsentRequest : FullAuditedEntity<long>, IMustHaveTenant
    {
        public int TenantId { get; set; }
        
        public string ConsentId { get; set; } // ABDM assigned Consent Artifact ID
        
        public long PatientId { get; set; }
        [ForeignKey(nameof(PatientId))]
        public virtual Patient Patient { get; set; }
        
        public string Status { get; set; } // GRANTED, DENIED, REVOKED, EXPIRED
        
        public DateTime? Expiration { get; set; }
        
        public string Purpose { get; set; } // Purpose of consent (e.g., Care Management)
    }
}
