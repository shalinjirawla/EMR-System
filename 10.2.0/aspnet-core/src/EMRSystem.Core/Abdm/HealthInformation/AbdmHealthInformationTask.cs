using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace EMRSystem.Abdm.HealthInformation
{
    public class AbdmHealthInformationTask : FullAuditedEntity<long>, IMustHaveTenant
    {
        public int TenantId { get; set; }
        
        public string TransactionId { get; set; } // ABDM assigned Transaction ID
        
        public string ConsentId { get; set; } // Reference to Consent
        
        public string Status { get; set; } // PENDING, PROCESSING, COMPLETED, FAILED
        
        public string EncryptedDataUrl { get; set; } // Internal path where we stored the encrypted bundle temporarily
        
        public string ErrorMessage { get; set; }
    }
}
