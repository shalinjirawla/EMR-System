using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMRSystem.Insurances.Dto
{
    public class CreateUpdatePatientInsuranceDto : EntityDto<long>
    {
        public int TenantId { get; set; }
        public long PatientId { get; set; }
        public long InsuranceId { get; set; }

        public string PolicyNumber { get; set; }
        public decimal CoverageLimit { get; set; }
        public decimal? CoPayPercentage { get; set; }

        public bool IsActive { get; set; } = true;
    }
}
