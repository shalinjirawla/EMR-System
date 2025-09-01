using Abp.Domain.Entities;

namespace EMRSystem.Doctors
{
    public class ConsultationRequests : Entity<long>, IMustHaveTenant
    {
        public int TenantId { get; set; }
        public long? PrescriptionId { get; set; }
        public virtual EMRSystem.Prescriptions.Prescription Prescriptions { get; set; }
        public long? RequestingDoctorId { get; set; }
        public EMRSystem.Doctors.Doctor RequestingDoctor { get; set; }
        public long? RequestedSpecialistId { get; set; }
        public EMRSystem.Doctors.Doctor RequestedSpecialist { get; set; }
        public Status Status { get; set; }
        public string Notes { get; set; }
        public string AdviceResponse { get; set; }
    }

    public enum Status
    {
        Pending, In_review, Completed
    }
}
