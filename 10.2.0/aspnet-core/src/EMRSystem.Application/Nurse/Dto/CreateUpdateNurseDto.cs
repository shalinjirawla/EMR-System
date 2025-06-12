using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMRSystem.Nurse.Dto
{
    public class CreateUpdateNurseDto : EntityDto<long>
    {
        public string FullName { get; set; }
        public string Gender { get; set; }
        public string ShiftTiming { get; set; }
        public string Department { get; set; }
        public string Qualification { get; set; }
        public int YearsOfExperience { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public long AbpUserId { get; set; }
    }
}
