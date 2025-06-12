using Abp.Application.Services.Dto;
using Abp.Application.Services;
using Abp.Domain.Repositories;
using EMRSystem.Authorization;
using EMRSystem.Billings;
using EMRSystem.BillingStaff.Dto;
using EMRSystem.BillingStaff;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EMRSystem.Appointments.Dto;
using EMRSystem.Nurse.Dto;
using EMRSystem.Nurse;

namespace EMRSystem.Appointments
{
    //AbpAuthorize(PermissionNames.Pages_Billing)]
    public class AppointmentAppService : AsyncCrudAppService<EMRSystem.Appointments.Appointment, AppointmentDto, long, PagedAndSortedResultRequestDto, CreateUpdateAppointmentDto, CreateUpdateAppointmentDto>,
  IAppointmentAppService
    {
        public AppointmentAppService(IRepository<EMRSystem.Appointments.Appointment, long> repository) : base(repository)
        {
        }
    }


}
