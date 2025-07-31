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
using EMRSystem.Doctor;
using EMRSystem.Patients;
using EMRSystem.Prescriptions.Dto;
using EMRSystem.Users.Dto;
using EMRSystem.Vitals.Dto;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;


namespace EMRSystem.Prescriptions
{
    public class PrescriptionAppService : AsyncCrudAppService<Prescription, PrescriptionDto, long, PagedPrescriptionResultRequestDto, CreateUpdatePrescriptionDto, CreateUpdatePrescriptionDto>,
   IPrescriptionAppService
    {
        private readonly IDoctorAppService _doctorAppService;
        private readonly IRepository<Appointment, long> _appointmentRepository;
        private readonly UserManager _userManager;
        public PrescriptionAppService(IRepository<Prescription, long> repository
            , IDoctorAppService doctorAppService, UserManager userManager, IRepository<Appointment, long> appointmentRepository
            ) : base(repository)
        {
            _doctorAppService = doctorAppService;
            _userManager = userManager;
            _appointmentRepository = appointmentRepository;
        }
        protected override IQueryable<Prescription> CreateFilteredQuery(PagedPrescriptionResultRequestDto input)
        {
            try
            {

            var userId = AbpSession.UserId;
            var doctor = _doctorAppService.GetDoctorDetailsByAbpUserID(userId.Value);

            return Repository
                .GetAll()
                .Include(x => x.Patient)
                .Include(x => x.Doctor)
                .Include(x => x.LabTests) 
                .WhereIf(!input.Keyword.IsNullOrWhiteSpace(), x =>
                    x.Diagnosis.Contains(input.Keyword) ||
                    x.Notes.Contains(input.Keyword) ||
                    x.Patient.FullName.Contains(input.Keyword) ||
                    x.Doctor.FullName.Contains(input.Keyword) ||
                    x.Items.Any(i => i.MedicineName.Contains(input.Keyword)))
                .WhereIf(input.FromDate.HasValue, x => x.IssueDate >= input.FromDate.Value)
                .WhereIf(input.ToDate.HasValue, x => x.IssueDate <= input.ToDate.Value)
                .WhereIf(doctor != null, x => x.Doctor.Id == doctor.Id)
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
                    }).ToList(),
                    LabTests = x.LabTests.Select(lt => new EMRSystem.LabReports.PrescriptionLabTest
                    {
                        Id = lt.Id,
                        LabReportsTypeId = lt.LabReportsTypeId,
                        TestStatus = lt.TestStatus,
                        CreatedDate = lt.CreatedDate
                    }).ToList()
                });
            }
            catch(SqlException sqlEx)
            {
                throw sqlEx;
            }
            catch (Exception ex)
            {
                throw ex;
            }
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
                new EMRSystem.LabReports.PrescriptionLabTest { LabReportsTypeId = id }).ToList();

            await Repository.InsertAsync(prescription);
            var appointment = await _appointmentRepository.GetAsync(input.AppointmentId);
            appointment.Status = AppointmentStatus.Completed;
            await _appointmentRepository.UpdateAsync(appointment);
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
                existingPrescription.LabTests.Add(new EMRSystem.LabReports.PrescriptionLabTest
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
            var query = await Repository.GetAllIncludingAsync(
                x => x.Patient,
                x => x.Doctor,
                x => x.Appointment,
                x => x.LabTests,
                x => x.Items); // Don't forget to include Items if needed

            var details = await query
                .Where(x => x.Id == id)
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
                    Appointment = x.Appointment == null ? null : new Appointment
                    {
                        Id = x.Appointment.Id,
                        AppointmentDate = x.Appointment.AppointmentDate,
                    },
                    Items = x.Items.Select(i => new PrescriptionItem
                    {
                        Id = i.Id,
                        MedicineName = i.MedicineName,
                        MedicineId=i.MedicineId,
                        Dosage = i.Dosage,
                        Frequency = i.Frequency,
                        Duration = i.Duration,
                        Instructions = i.Instructions,
                    }).ToList(),
                    LabTests = x.LabTests.Select(lt => new EMRSystem.LabReports.PrescriptionLabTest
                    {
                        Id = lt.Id,
                        LabReportsTypeId = lt.LabReportsTypeId,
                        TestStatus = lt.TestStatus,
                        CreatedDate = lt.CreatedDate
                    }).ToList()
                })
                .FirstOrDefaultAsync();

            if (details == null)
            {
                throw new EntityNotFoundException(typeof(Prescription), id);
            }

            var prescription = ObjectMapper.Map<CreateUpdatePrescriptionDto>(details);
            prescription.LabTestIds = details.LabTests.Select(lt => (int)lt.LabReportsTypeId).ToList();

            return prescription;
        }

        // PrescriptionAppService.cs
        public async Task<PrescriptionViewDto> GetPrescriptionForView(long id)
        {
            var prescription = await Repository.GetAll()
                .Include(p => p.Patient)
                .Include(p => p.Doctor)
                .Include(p => p.Appointment)
                .Include(p => p.Items)
                .Include(p => p.LabTests)
                    .ThenInclude(lt => lt.LabReportsType)
                .Where(p => p.Id == id)
                .Select(p => new
                {
                    Prescription = p,
                    Patient = p.Patient,
                    Doctor = p.Doctor,
                    Appointment = p.Appointment,
                    Items = p.Items,
                    LabTests = p.LabTests.Select(lt => new
                    {
                        lt.LabReportsType.ReportType,
                        lt.TestStatus,
                        lt.CreatedDate
                    })
                })
                .FirstOrDefaultAsync();

            if (prescription == null)
            {
                throw new EntityNotFoundException(typeof(Prescription), id);
            }

            return new PrescriptionViewDto
            {
                Id = prescription.Prescription.Id,
                Diagnosis = prescription.Prescription.Diagnosis,
                Notes = prescription.Prescription.Notes,
                IssueDate = prescription.Prescription.IssueDate,
                IsFollowUpRequired = prescription.Prescription.IsFollowUpRequired,
                AppointmentDate = prescription.Appointment?.AppointmentDate,

                // Patient Info
                PatientFullName = prescription.Patient?.FullName,
                PatientDateOfBirth = prescription.Patient?.DateOfBirth ?? DateTime.MinValue,
                PatientGender = prescription.Patient?.Gender,
                PatientBloodGroup = prescription.Patient?.BloodGroup,

                // Doctor Info
                DoctorFullName = prescription.Doctor?.FullName,
                DoctorSpecialization = prescription.Doctor?.Specialization,
                DoctorRegistrationNumber = prescription.Doctor?.RegistrationNumber,

                // Medications
                Items = prescription.Items.Select(i => new PrescriptionItemViewDto
                {
                    MedicineName = i.MedicineName,
                    Dosage = i.Dosage,
                    Frequency = i.Frequency,
                    Duration = i.Duration,
                    Instructions = i.Instructions,
                    MedicineId = i.MedicineId
                }).ToList(),

                // Lab Tests
                LabTests = prescription.LabTests.Select(lt => new PrescriptionLabTestViewDto
                {
                    ReportTypeName = lt.ReportType
                }).ToList()
            };
        }

        [HttpGet]
        public async Task<List<string>> GetCurrentUserRolesAsync()
        {
            if (!AbpSession.UserId.HasValue)
                return null;
            var user = await _userManager.GetUserByIdAsync(AbpSession.UserId.Value);
            var roles = await _userManager.GetRolesAsync(user);
            return roles.ToList();
        }
    }
}