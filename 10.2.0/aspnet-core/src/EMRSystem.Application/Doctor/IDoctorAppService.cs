using Abp.Application.Services.Dto;
using Abp.Application.Services;
using EMRSystem.BillingStaff.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EMRSystem.Doctor.Dto;

namespace EMRSystem.Doctor
{
    public interface IDoctorAppService : IAsyncCrudAppService<
    DoctorDto, long, PagedAndSortedResultRequestDto, CreateUpdateDoctorDto, CreateUpdateDoctorDto>
    {
    }
}
