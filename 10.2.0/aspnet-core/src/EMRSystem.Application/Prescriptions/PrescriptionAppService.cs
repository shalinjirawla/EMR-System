using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Domain.Entities;
using Abp.Domain.Repositories;
using Abp.Extensions;
using Abp.Linq.Extensions;
using Abp.UI;
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
                .WhereIf(!input.Keyword.IsNullOrWhiteSpace(), x =>
                    x.Diagnosis.Contains(input.Keyword) ||
                    x.Notes.Contains(input.Keyword) ||
                    x.Patient.FullName.Contains(input.Keyword) ||
                    x.Doctor.FullName.Contains(input.Keyword) ||
                    x.Items.Any(i => i.MedicineName.Contains(input.Keyword)))
                .WhereIf(input.FromDate.HasValue, x => x.IssueDate >= input.FromDate.Value)
                .WhereIf(input.ToDate.HasValue, x => x.IssueDate <= input.ToDate.Value)
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

            prescription.Items = input.Items.Select(item =>
                ObjectMapper.Map<PrescriptionItem>(item)).ToList();

            prescription.LabTests = input.LabTestIds.Select(id =>
                new EMRSystem.PrescriptionLabTests.PrescriptionLabTest { LabReportsTypeId = id }).ToList();

            await Repository.InsertAsync(prescription);
            await CurrentUnitOfWork.SaveChangesAsync();
        }
        public async Task UpdatePrescriptionWithItemAsync(CreateUpdatePrescriptionDto input)
        {
            // First get the existing prescription with its items
            var existingPrescription = await Repository.GetAllIncluding(
                p => p.Items,
                p => p.LabTests
            ).FirstOrDefaultAsync(p => p.Id == input.Id);

            if (existingPrescription == null)
            {
                throw new UserFriendlyException("Prescription not found");
            }

            // Map the input to the existing prescription
            ObjectMapper.Map(input, existingPrescription);

            // Handle Items update
            var existingItems = existingPrescription.Items.ToList();

            // Update existing items or add new ones
            foreach (var inputItem in input.Items)
            {
                var existingItem = existingItems.FirstOrDefault(i => i.Id == inputItem.Id);
                if (existingItem != null)
                {
                    // Update existing item
                    ObjectMapper.Map(inputItem, existingItem);
                }
                else
                {
                    // Add new item
                    var newItem = ObjectMapper.Map<PrescriptionItem>(inputItem);
                    existingPrescription.Items.Add(newItem);
                }
            }

            // Remove items that are no longer in the input
            var itemsToRemove = existingItems.Where(ei => !input.Items.Any(ii => ii.Id == ei.Id)).ToList();
            foreach (var item in itemsToRemove)
            {
                existingPrescription.Items.Remove(item);
            }

            // Handle LabTests update
            existingPrescription.LabTests.Clear();
            foreach (var labTestId in input.LabTestIds)
            {
                existingPrescription.LabTests.Add(new  EMRSystem.PrescriptionLabTests.PrescriptionLabTest
                {
                    LabReportsTypeId = labTestId,
                    PrescriptionId = input.Id
                });
            }

            await Repository.UpdateAsync(existingPrescription);
            await CurrentUnitOfWork.SaveChangesAsync();
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
