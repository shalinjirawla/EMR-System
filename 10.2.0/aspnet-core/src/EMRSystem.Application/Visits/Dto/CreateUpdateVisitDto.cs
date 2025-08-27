using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMRSystem.Visits.Dto
{
    public class CreateUpdateVisitDto : EntityDto<long>
    {
        public int TenantId { get; set; }
        public long PatientId { get; set; }
        //public long DepartmentId { get; set; }
        public long DoctorId { get; set; }
        public long NurseId { get; set; }
        public DateTime DateOfVisit { get; set; }
        public TimeSpan TimeOfVisit { get; set; }
        public string ReasonForVisit { get; set; }
        public PaymentMode PaymentMode { get; set; }
        public decimal? ConsultationFee { get; set; }
    }
}
