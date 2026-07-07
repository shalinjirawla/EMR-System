using Abp.Domain.Entities.Auditing;
using EMRSystem.Patients;
using System.ComponentModel.DataAnnotations.Schema;

namespace EMRSystem.Abdm.Abha
{
    [Table("PatientAbhaDetails")]
    public class PatientAbhaDetails : FullAuditedEntity<long>
    {
        public long PatientId { get; set; }
        public string AbhaNumber { get; set; }
        public string AbhaAddress { get; set; }
        public string EnrolmentReferenceId { get; set; }
        public string QrCodeBase64 { get; set; }
        public string CardBase64 { get; set; }
        
        [ForeignKey(nameof(PatientId))]
        public virtual Patient Patient { get; set; }
    }
}
