using Abp.Application.Services.Dto;
using Abp.Application.Services;
using EMRSystem.Nurse.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EMRSystem.Patient_Discharge.Dto;
using EMRSystem.Users.Dto;

namespace EMRSystem.Patient_Discharge
{
    public interface IPatientDischargeAppService : IAsyncCrudAppService<
    PatientDischargeDto, long, PagedPatientDischargeResultRequestDto, CreateUpdatePatientDischargeDto, CreateUpdatePatientDischargeDto>
    {
    }
}
