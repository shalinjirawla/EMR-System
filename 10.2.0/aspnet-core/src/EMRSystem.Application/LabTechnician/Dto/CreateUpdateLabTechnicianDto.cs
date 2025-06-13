using Abp.Application.Services.Dto;
using EMRSystem.LabReports;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMRSystem.LabTechnician.Dto
{
    public class CreateUpdateLabTechnicianDto : EntityDto<long>
    {
        public int TenantId { get; set; }

        public string FullName { get; set; }
        public string Gender { get; set; }
        public string Qualification { get; set; }
        public int YearsOfExperience { get; set; }
        public LabDepartment Department { get; set; }
        public string CertificationNumber { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public long AbpUserId { get; set; }
    }
}
