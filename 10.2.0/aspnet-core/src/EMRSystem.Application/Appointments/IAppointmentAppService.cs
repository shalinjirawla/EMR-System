using Abp.Application.Services.Dto;
using Abp.Application.Services;
using EMRSystem.BillingStaff.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EMRSystem.Appointments.Dto;
using EMRSystem.Nurse.Dto;

namespace EMRSystem.Appointments
{
    public interface IAppointmentAppService : IAsyncCrudAppService<
   AppointmentDto, long, PagedAndSortedResultRequestDto, CreateUpdateAppointmentDto, CreateUpdateAppointmentDto>
    {
    }
}
