using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Domain.Entities;
using Abp.Domain.Repositories;
using EMRSystem.Appointments;
using EMRSystem.Appointments.Dto;
using EMRSystem.Authorization.Users;
using EMRSystem.Patients;
using EMRSystem.Prescriptions.Dto;
using EMRSystem.Users.Dto;
using EMRSystem.Vitals.Dto;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;


namespace EMRSystem.Prescriptions
{
    public class PrescriptionAppService : AsyncCrudAppService<Prescription, PrescriptionDto, long, PagedPrescriptionResultRequestDto, CreateUpdatePrescriptionDto, CreateUpdatePrescriptionDto>,
   IPrescriptionAppService
    {
        public PrescriptionAppService(IRepository<Prescription, long> repository
            ) : base(repository)
        {
        }
        protected override IQueryable<Prescription> CreateFilteredQuery(PagedPrescriptionResultRequestDto input)
        {
            return Repository
                .GetAll()
                .Include(x => x.Patient)
                .Include(x => x.Doctor)
                .Select(x => new Prescription
                {
                    Id = x.Id,
                    TenantId = x.TenantId,
                    Diagnosis = x.Diagnosis,
                    Notes = x.Notes,
                    IssueDate = x.IssueDate,
                    IsFollowUpRequired = x.IsFollowUpRequired,
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
                    Items = x.Items.Select(i => new PrescriptionItem
                    {
                        Id = i.Id,
                        MedicineName = i.MedicineName,
                        Dosage = i.Dosage,
                        Frequency = i.Frequency,
                        Duration = i.Duration,
                        Instructions = i.Instructions,
                    }).ToList()
                });
        }

        protected override IQueryable<Prescription> ApplySorting(IQueryable<Prescription> query, PagedPrescriptionResultRequestDto input)
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

        public async Task CreatePrescriptionWithItemAsync(CreateUpdatePrescriptionDto input)
        {
            var prescription = ObjectMapper.Map<Prescription>(input);
            await Repository.InsertAndGetIdAsync(prescription);
            CurrentUnitOfWork.SaveChanges();
        }
        public async Task UpdatePrescriptionWithItemAsync(CreateUpdatePrescriptionDto input)
        {
            var prescription = ObjectMapper.Map<Prescription>(input);
            await Repository.UpdateAsync(prescription);
            CurrentUnitOfWork.SaveChanges();
        }

        public async Task<CreateUpdatePrescriptionDto> GetPrescriptionDetailsById(long id)
        {
            var details = await Repository.GetAllIncludingAsync(x => x.Patient, x => x.Doctor, x => x.Appointment).Result
                .Select(x => new Prescription
                {
                    Id = x.Id,
                    TenantId = x.TenantId,
                    Diagnosis = x.Diagnosis,
                    Notes = x.Notes,
                    IssueDate = x.IssueDate,
                    IsFollowUpRequired = x.IsFollowUpRequired,
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
                    Appointment = x.Appointment == null ? null : new EMRSystem.Appointments.Appointment
                    {
                        Id = x.Appointment.Id,
                        AppointmentDate = x.Appointment.AppointmentDate,
                        StartTime = x.Appointment.StartTime,
                        EndTime = x.Appointment.EndTime,
                    },
                    Items = x.Items.Select(i => new PrescriptionItem
                    {
                        Id = i.Id,
                        MedicineName = i.MedicineName,
                        Dosage = i.Dosage,
                        Frequency = i.Frequency,
                        Duration = i.Duration,
                        Instructions = i.Instructions,
                    }).ToList()
                })
                .FirstOrDefaultAsync(x => x.Id == id);

            if (details == null)
            {
                throw new EntityNotFoundException(typeof(Prescription), id);
            }
            var prescription = ObjectMapper.Map<CreateUpdatePrescriptionDto>(details);
            return prescription;
        }
    }
}
