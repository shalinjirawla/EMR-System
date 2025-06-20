using Abp.Application.Services;
using Abp.Domain.Repositories;
using System.Collections.Generic;
using System.Linq;
using EMRSystem.Appointments.Dto;
using Abp.Application.Services.Dto;
using Microsoft.EntityFrameworkCore;
using EMRSystem.Patients;
using EMRSystem.Prescriptions;
using Abp.Domain.Entities;
using EMRSystem.Prescriptions.Dto;
using System.Threading.Tasks;
using EMRSystem.Authorization.Users;
using Abp.Collections.Extensions;
using Abp.Extensions;
using System;
using EMRSystem.Users.Dto;

namespace EMRSystem.Appointments
{
    //AbpAuthorize(PermissionNames.Pages_Billing)]
    public class AppointmentAppService : AsyncCrudAppService<EMRSystem.Appointments.Appointment, AppointmentDto, long, PagedAppoinmentResultRequestDto, CreateUpdateAppointmentDto, CreateUpdateAppointmentDto>,
  IAppointmentAppService
    {
        public AppointmentAppService(IRepository<EMRSystem.Appointments.Appointment, long> repository) : base(repository)
        {
        }

        protected override IQueryable<Appointment> CreateFilteredQuery(PagedAppoinmentResultRequestDto input)
        {
            var res = Repository
                 .GetAll()
                 .Include(x => x.Patient)
                 .Include(x => x.Doctor)
                 .Include(x => x.Nurse)
                 .Select(x => new Appointment
                 {
                     Id = x.Id,
                     TenantId = x.TenantId,
                     AppointmentDate = x.AppointmentDate,
                     StartTime = x.StartTime,
                     EndTime = x.EndTime,
                     ReasonForVisit = x.ReasonForVisit,
                     Status = x.Status,
                     IsFollowUp = x.IsFollowUp,
                     PatientId = x.PatientId,
                     DoctorId = x.DoctorId,
                     NurseId = x.NurseId,
                     Patient = x.Patient == null ? null : new Patient
                     {
                         Id = x.Patient.Id,
                         FullName = x.Patient.FullName
                     },
                     Doctor = x.Doctor == null ? null : new EMRSystem.Doctors.Doctor
                     {
                         Id = x.Doctor.Id,
                         FullName = x.Doctor.FullName
                     },
                     Nurse = x.Nurse == null ? null : new EMRSystem.Nurses.Nurse
                     {
                         Id = x.Nurse.Id,
                         FullName = x.Nurse.FullName
                     },
                 })
                 .WhereIf(!input.Keyword.IsNullOrWhiteSpace(), x => x.Patient.FullName.Contains(input.Keyword) || x.Doctor.FullName.Contains(input.Keyword))
                 .WhereIf(input.Status.HasValue, i => i.Status == input.Status.Value);
            return res.AsQueryable();
        }
        protected override IQueryable<Appointment> ApplySorting(IQueryable<Appointment> query, PagedAppoinmentResultRequestDto input)
        {
            return base.ApplySorting(query, input);
        }
        public ListResultDto<AppointmentDto> GetPatientAppointment(long patientId, long doctorId)
        {
            var appointments = Repository.GetAllIncluding(x => x.Patient)
                 .Where(x => x.PatientId == patientId && x.DoctorId == doctorId)
                 .ToList();

            var mapped = ObjectMapper.Map<List<AppointmentDto>>(appointments);
            return new ListResultDto<AppointmentDto>(mapped);
        }
        public async Task<CreateUpdateAppointmentDto> GetAppointmentDetailsById(long id)
        {
            var details = await Repository.GetAllIncludingAsync(x => x.Patient, x => x.Doctor, x => x.Nurse).Result
              .Select(x => new Appointment
              {
                  Id = x.Id,
                  TenantId = x.TenantId,
                  AppointmentDate = x.AppointmentDate,
                  StartTime = x.StartTime,
                  EndTime = x.EndTime,
                  ReasonForVisit = x.ReasonForVisit,
                  Status = x.Status,
                  IsFollowUp = x.IsFollowUp,
                  PatientId = x.PatientId,
                  DoctorId = x.DoctorId,
                  NurseId = x.NurseId,
                  Patient = x.Patient == null ? null : new Patient
                  {
                      Id = x.Patient.Id,
                      FullName = x.Patient.FullName
                  },
                  Doctor = x.Doctor == null ? null : new EMRSystem.Doctors.Doctor
                  {
                      Id = x.Doctor.Id,
                      FullName = x.Doctor.FullName
                  },
                  Nurse = x.Nurse == null ? null : new EMRSystem.Nurses.Nurse
                  {
                      Id = x.Nurse.Id,
                      FullName = x.Nurse.FullName
                  },
              })
                .FirstOrDefaultAsync(x => x.Id == id);

            if (details == null)
            {
                throw new EntityNotFoundException(typeof(Appointment), id);
            }
            var prescription = ObjectMapper.Map<CreateUpdateAppointmentDto>(details);
            return prescription;
        }
        public async Task CreateAppoinment(CreateUpdateAppointmentDto dto)
        {
            var appointment = ObjectMapper.Map<Appointment>(dto);
            await Repository.InsertAsync(appointment);
            CurrentUnitOfWork.SaveChanges();
        }
        public async Task UpdateAppoinment(CreateUpdateAppointmentDto input)
        {
            var appointment = ObjectMapper.Map<Appointment>(input);
            await Repository.UpdateAsync(appointment);
            CurrentUnitOfWork.SaveChanges();
        }
    }
}
