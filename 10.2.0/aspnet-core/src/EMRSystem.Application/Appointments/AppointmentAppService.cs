using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Collections.Extensions;
using Abp.Domain.Entities;
using Abp.Domain.Repositories;
using Abp.Extensions;
using EMRSystem.Appointments.Dto;
using EMRSystem.Authorization.Users;
using EMRSystem.Patients;
using EMRSystem.Prescriptions;
using EMRSystem.Prescriptions.Dto;
using EMRSystem.Users.Dto;
using EMRSystem.Vitals;
using EMRSystem.Vitals.Dto;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Linq.Dynamic.Core;


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
            // Start with base query without projection
            var baseQuery = Repository
                .GetAll()
                .Include(x => x.Patient)
                .Include(x => x.Doctor)
                .Include(x => x.Nurse);

            var filteredQuery = baseQuery.AsQueryable(); // Explicitly convert to IQueryable

            if (!input.Keyword.IsNullOrWhiteSpace())
            {
                filteredQuery = filteredQuery.Where(x =>
                    (x.Patient.FullName != null && x.Patient.FullName.Contains(input.Keyword)) ||
                    (x.Doctor.FullName != null && x.Doctor.FullName.Contains(input.Keyword)));
            }

            if (input.Status.HasValue)
            {
                filteredQuery = filteredQuery.Where(x => x.Status == input.Status.Value);
            }

            // Apply projection after all filtering
            var result = filteredQuery.Select(x => new Appointment
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
            });

            return result;
        }
        protected override IQueryable<Appointment> ApplySorting(IQueryable<Appointment> query, PagedAppoinmentResultRequestDto input)
        {
            if (!string.IsNullOrWhiteSpace(input.Sorting))
            {
                var sorting = input.Sorting;

                if (sorting.Contains("patientName", StringComparison.OrdinalIgnoreCase))
                    sorting = sorting.Replace("patientName", "Patient.FullName", StringComparison.OrdinalIgnoreCase);

                if (sorting.Contains("doctorName", StringComparison.OrdinalIgnoreCase))
                    sorting = sorting.Replace("doctorName", "Doctor.FullName", StringComparison.OrdinalIgnoreCase);

                return query.OrderBy(sorting);
            }

            return base.ApplySorting(query, input);
        }
        public ListResultDto<AppointmentDto> GetPatientAppointment(long patientId, long doctorId)
        {
            var appointments = Repository.GetAllIncluding(x => x.Patient)
                    .WhereIf(doctorId > 0, x => x.DoctorId == doctorId)
                    .Where(x => x.PatientId == patientId)
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
