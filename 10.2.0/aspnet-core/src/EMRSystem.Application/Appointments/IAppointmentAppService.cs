using Abp.Application.Services;
using Abp.Application.Services.Dto;
using EMRSystem.AppointmentReceipt.Dto;
using EMRSystem.Appointments.Dto;
using EMRSystem.BillingStaff.Dto;
using EMRSystem.Nurse.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMRSystem.Appointments
{
    public interface IAppointmentAppService : IAsyncCrudAppService<
   AppointmentDto, long, PagedAppoinmentResultRequestDto, CreateUpdateAppointmentDto, CreateUpdateAppointmentDto>
    {
        Task<AppointmentReceiptDto> GenerateAppointmentReceipt(long appointmentId, string paymentMethod);
    }
}
