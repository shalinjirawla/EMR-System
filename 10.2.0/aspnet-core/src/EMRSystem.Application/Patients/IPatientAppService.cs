using Abp.Application.Services;
using Abp.Application.Services.Dto;
using EMRSystem.Patients.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMRSystem.Patients
{
    public interface IPatientAppService : IAsyncCrudAppService<
    PatientDto, long, PagedAndSortedResultRequestDto, CreateUpdatePatientDto, CreateUpdatePatientDto>{}
}
