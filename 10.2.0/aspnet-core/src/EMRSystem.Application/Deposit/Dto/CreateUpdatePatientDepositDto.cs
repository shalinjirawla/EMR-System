using EMRSystem.Invoices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abp.Application.Services.Dto;

namespace EMRSystem.Deposit.Dto
{
    public class CreateUpdatePatientDepositDto : EntityDto<long>
    {
        public int TenantId { get; set; }
        public long PatientId { get; set; }
    }
}
