using Abp.Application.Services.Dto;
using EMRSystem.Admission;
using EMRSystem.Patients;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMRSystem.Admissions.Dto
{
    public class CreateUpdateAdmissionDto : EntityDto<long>
    {
        public int TenantId { get; set; }
        public long PatientId { get; set; }
        public DateTime AdmissionDateTime { get; set; }
        public long DoctorId { get; set; }
        public long? NurseId { get; set; }
        public long? RoomId { get; set; }
        public long? BedId { get; set; }

        public AdmissionType AdmissionType { get; set; }

        //Signatures


    }
}
