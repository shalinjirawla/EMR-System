using Abp.Application.Services.Dto;
using EMRSystem.Departments;
using EMRSystem.Departments.Dto;
using EMRSystem.Doctor.Dto;
using EMRSystem.Patients;
using EMRSystem.Patients.Dto;
using EMRSystem.Nurse.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMRSystem.Visits.Dto
{
    public class VisitListDto : EntityDto<long>
    {
        public virtual PatientDto Patient { get; set; }
        public virtual DepartmentListDto Department { get; set; }
        public virtual DoctorDto Doctor { get; set; }
        public virtual NurseDto Nurse { get; set; }
        public DateTime DateOfVisit { get; set; }
        public TimeSpan TimeOfVisit { get; set; }
        public string ReasonForVisit { get; set; }
        public PaymentMode PaymentMode { get; set; }
        public decimal? ConsultationFee { get; set; }
    }
}
