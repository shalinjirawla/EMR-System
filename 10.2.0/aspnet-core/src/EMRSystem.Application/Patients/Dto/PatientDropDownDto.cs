using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMRSystem.Patients.Dto
{
    public class PatientDropDownDto : EntityDto<long>
    {
        public string FullName { get; set; }
        public bool IsAdmitted { get; set; }

    }
}
