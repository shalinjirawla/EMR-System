using Abp.Application.Services;
using Abp.Domain.Repositories;
using System.Collections.Generic;
using System.Linq;
using EMRSystem.Appointments.Dto;
using Abp.Application.Services.Dto;
using Microsoft.EntityFrameworkCore;

namespace EMRSystem.Appointments
{
    //AbpAuthorize(PermissionNames.Pages_Billing)]
    public class AppointmentAppService : AsyncCrudAppService<EMRSystem.Appointments.Appointment, AppointmentDto, long, PagedAndSortedResultRequestDto, CreateUpdateAppointmentDto, CreateUpdateAppointmentDto>,
  IAppointmentAppService
    {
        public AppointmentAppService(IRepository<EMRSystem.Appointments.Appointment, long> repository) : base(repository)
        {
        }

        public ListResultDto<AppointmentDto> GetPatientAppointment(long patientId, long doctorId)
        {
            var appointments =  Repository.GetAllIncluding(x => x.Patient)
                 .Where(x => x.PatientId == patientId && x.DoctorId == doctorId)
                 .ToList();

            var mapped = ObjectMapper.Map<List<AppointmentDto>>(appointments);
            return new ListResultDto<AppointmentDto>(mapped);
        }
    }


}
