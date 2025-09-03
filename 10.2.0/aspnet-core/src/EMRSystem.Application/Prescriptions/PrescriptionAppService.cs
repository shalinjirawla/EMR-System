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
using EMRSystem.Doctor.Dto;
using EMRSystem.LabReports;
using EMRSystem.Patients;
using EMRSystem.Patients.Dto;
using EMRSystem.PrescriptionLabTest.Dto;
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
using EMRSystem.EmergencyProcedure;
using EMRSystem.IpdChargeEntry;
using Stripe.V2;
using Abp.EntityFrameworkCore.Repositories;
using EMRSystem.Pharmacist.Dto;
using EMRSystem.MedicineOrder;


namespace EMRSystem.Prescriptions
{
    public class PrescriptionAppService : AsyncCrudAppService<Prescription, PrescriptionDto, long, PagedPrescriptionResultRequestDto, CreateUpdatePrescriptionDto, CreateUpdatePrescriptionDto>,
   IPrescriptionAppService
    {
        private readonly IDoctorAppService _doctorAppService;
        private readonly IRepository<Appointment, long> _appointmentRepository;
        private readonly IRepository<Patient, long> _patientRepository;
        private readonly IRepository<EMRSystem.LabReports.PrescriptionLabTest, long> _prescriptionLabTestRepository;
        private readonly IRepository<EMRSystem.EmergencyChargeEntries.EmergencyChargeEntry, long> _emergencyChargeEntriesRepository;
        private readonly IRepository<EMRSystem.DoctorMaster.DoctorMaster, long> _doctorMasterRepository;
        private readonly IRepository<EMRSystem.EmergencyProcedure.EmergencyProcedure, long> _emergencyProcedureRepository;
        private readonly IRepository<EMRSystem.Doctors.ConsultationRequests, long> _consultationRequestsRepository;
        private readonly IRepository<EMRSystem.Pharmacists.PharmacistPrescriptions, long> _pharmacistPrescriptionsRepository;
        private readonly IRepository<EMRSystem.Pharmacists.PharmacistPrescriptionsItem, long> _pharmacistPrescriptionsItemRepository;
        private readonly IRepository<EMRSystem.Pharmacists.PharmacistInventory, long> _pharmacistInventoryRepository;

        private readonly UserManager _userManager;
        public PrescriptionAppService(IRepository<Prescription, long> repository
            , IDoctorAppService doctorAppService, UserManager userManager, IRepository<Appointment, long> appointmentRepository,
            IRepository<Patient, long> patientRepository, IRepository<EMRSystem.LabReports.PrescriptionLabTest, long> prescriptionLabTestRepository
            , IRepository<EmergencyChargeEntries.EmergencyChargeEntry, long> emergencyChargeEntriesRepository,
            IRepository<DoctorMaster.DoctorMaster, long> doctorMasterRepository,
            IRepository<EmergencyProcedure.EmergencyProcedure, long> emergencyProcedureRepository,
            IRepository<Doctors.ConsultationRequests, long> consultationRequestsRepository,
            IRepository<EMRSystem.Pharmacists.PharmacistPrescriptions, long> pharmacistPrescriptionsRepository,
            IRepository<Pharmacists.PharmacistInventory, long> pharmacistInventoryRepository,
            IRepository<EMRSystem.Pharmacists.PharmacistPrescriptionsItem, long> pharmacistPrescriptionsItemRepository) : base(repository)
        {
            _doctorAppService = doctorAppService;
            _userManager = userManager;
            _appointmentRepository = appointmentRepository;
            _patientRepository = patientRepository;
            _prescriptionLabTestRepository = prescriptionLabTestRepository;
            _emergencyChargeEntriesRepository = emergencyChargeEntriesRepository;
            _doctorMasterRepository = doctorMasterRepository;
            _emergencyProcedureRepository = emergencyProcedureRepository;
            _consultationRequestsRepository = consultationRequestsRepository;
            _pharmacistPrescriptionsRepository = pharmacistPrescriptionsRepository;
            _pharmacistInventoryRepository = pharmacistInventoryRepository;
            _pharmacistPrescriptionsItemRepository = pharmacistPrescriptionsItemRepository;
        }
        protected override IQueryable<Prescription> CreateFilteredQuery(PagedPrescriptionResultRequestDto input)
        {
            try
            {

                var userId = AbpSession.UserId;
                var doctor = _doctorAppService.GetDoctorDetailsByAbpUserID(userId.Value);

                var dataa = Repository
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
                        IsEmergencyPrescription = x.IsEmergencyPrescription,
                        EmergencyCaseId = x.EmergencyCaseId,
                        Patient = x.Patient == null ? null : new Patient
                        {
                            Id = x.Patient.Id,
                            FullName = x.Patient.FullName,
                            IsAdmitted = x.Patient.IsAdmitted

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
                return dataa;
            }
            catch (SqlException sqlEx)
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
            // Map Prescription
            var prescription = ObjectMapper.Map<Prescription>(input);

            // Map Items
            prescription.Items = input.Items
                .Select(item =>
                {
                    item.Qty = CalculateQty(item.Frequency, item.Duration);
                    return ObjectMapper.Map<PrescriptionItem>(item);
                }).ToList();

            if (input.EmergencyProcedures != null && input.EmergencyProcedures.Any())
            {
                prescription.SelectedEmergencyProcedureses = input.EmergencyProcedures
                    .Select(itm => ObjectMapper.Map<SelectedEmergencyProcedures>(itm))
                    .ToList();
            }

            // Save prescription (to get Id)
            await Repository.InsertAsync(prescription);
            await CurrentUnitOfWork.SaveChangesAsync(); // Needed to get prescription.Id

            bool isAdmitted = false;
            // Fetch patient to check admission
            if (input.PatientId.HasValue)
            {
                var patient = await _patientRepository.GetAsync(input.PatientId.Value);
                isAdmitted = patient.IsAdmitted;
            }

            if (isAdmitted)
            {
                // Create and save each PrescriptionLabTest
                foreach (var labTestId in input.LabTestIds)
                {
                    var labTest = new EMRSystem.LabReports.PrescriptionLabTest
                    {
                        TenantId = input.TenantId,
                        PrescriptionId = prescription.Id,
                        LabReportsTypeId = labTestId,
                        IsPaid = isAdmitted,
                        TestStatus = LabTestStatus.Pending,
                        IsPrescribed = true,
                        IsFromPackage = false,
                        CreatedDate = DateTime.Now,
                        PatientId = input.PatientId
                    };

                    await _prescriptionLabTestRepository.InsertAsync(labTest);
                }
                // create Pharmacist Prescriptions
                {
                    var dto = new EMRSystem.Pharmacists.PharmacistPrescriptions();
                    dto.TenantId = input.TenantId;
                    dto.PrescriptionId = prescription.Id;
                    dto.IssueDate = DateTime.Now;
                    dto.Order_Status = OrderStatus.Pending;
                    var res = await _pharmacistPrescriptionsRepository.InsertAndGetIdAsync(dto);

                    // create Pharmacist Prescriptions Item
                    var dto2 = new EMRSystem.Pharmacists.PharmacistPrescriptionsItem();
                    dto2.TenantId = input.TenantId;
                    dto2.PharmacistPrescriptionId = res;
                    dto2.GrandTotal = await GetGrandTotal(prescription.Id);
                    dto2.CreatedAt = DateTime.Now;
                    await _pharmacistPrescriptionsItemRepository.InsertAsync(dto2);
                }
            }
            else
            {
                // Create and save each PrescriptionLabTest
                foreach (var labTestId in input.LabTestIds)
                {
                    var labTest = new EMRSystem.LabReports.PrescriptionLabTest
                    {
                        TenantId = input.TenantId,
                        PrescriptionId = prescription.Id,
                        LabReportsTypeId = labTestId,
                        IsPaid = input.IsEmergencyPrescription,
                        IsPrescribed = true,
                        IsFromPackage = false,
                        TestStatus = LabTestStatus.Pending,
                        CreatedDate = DateTime.Now,
                        IsEmergencyPrescription = input.IsEmergencyPrescription,
                        EmergencyCaseId = input.EmergencyCaseId,
                        PatientId = input.PatientId
                    };

                    await _prescriptionLabTestRepository.InsertAsync(labTest);
                }
            }

            await CreateUpdateCharges(input, prescription.Id, true);

            // Mark appointment as completed
            if (input.AppointmentId.HasValue)
            {
                var appointment = await _appointmentRepository.GetAsync(input.AppointmentId.Value);
                appointment.Status = AppointmentStatus.Completed;
                await _appointmentRepository.UpdateAsync(appointment);
            }

            if (input.IsSpecialAdviceRequired)
            {
                input.CreateUpdateConsultationRequests.PrescriptionId = prescription.Id;
                await CreateConsultationRequest(input.CreateUpdateConsultationRequests);
            }
            await CurrentUnitOfWork.SaveChangesAsync();
        }

        public async Task UpdatePrescriptionWithItemAsync(CreateUpdatePrescriptionDto input)
        {
            // First get the existing prescription with its items
            var existingPrescription = await Repository.GetAllIncluding(
                p => p.Items,
                p => p.LabTests,
                p => p.SelectedEmergencyProcedureses
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
                inputItem.Qty = CalculateQty(inputItem.Frequency, inputItem.Duration);
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
                    PrescriptionId = input.Id,
                    IsEmergencyPrescription = input.IsEmergencyPrescription,
                    EmergencyCaseId = input.EmergencyCaseId,
                    IsPaid = input.IsEmergencyPrescription,
                    PatientId = input.PatientId,
                });
            }


            // Handle procedure update
            var existingProcedure = existingPrescription.SelectedEmergencyProcedureses.ToList();
            // Update existing items or add new ones
            if (input.EmergencyProcedures != null && input.EmergencyProcedures.Any())
            {
                foreach (var inputItem2 in input.EmergencyProcedures)
                {
                    var existingItem2 = existingProcedure.FirstOrDefault(i => i.Id == inputItem2.EmergencyProcedureId);
                    if (existingItem2 != null)
                    {
                        ObjectMapper.Map(inputItem2, existingItem2);
                    }
                    else
                    {
                        var newItem2 = ObjectMapper.Map<EMRSystem.EmergencyProcedure.SelectedEmergencyProcedures>(inputItem2);
                        existingPrescription.SelectedEmergencyProcedureses.Add(newItem2);
                    }
                }
            }
            // Remove items that are no longer in the input
            var itemsToRemove2 = existingProcedure.Where(ei => !input.EmergencyProcedures.Any(ii => ii.EmergencyProcedureId == ei.Id)).ToList();
            foreach (var item2 in itemsToRemove2)
            {
                existingPrescription.SelectedEmergencyProcedureses.Remove(item2);
            }

            await Repository.UpdateAsync(existingPrescription);
            await CurrentUnitOfWork.SaveChangesAsync();

            if (input.IsSpecialAdviceRequired)
            {
                input.CreateUpdateConsultationRequests.PrescriptionId = existingPrescription.Id;
                await CreateConsultationRequest(input.CreateUpdateConsultationRequests);
            }

            await CreateUpdateCharges(input, input.Id, false);

        }

        public async Task<CreateUpdatePrescriptionDto> GetPrescriptionDetailsById(long id)
        {
            var query = await Repository.GetAllIncludingAsync(
                x => x.Patient,
                x => x.Doctor,
                x => x.Appointment,
                x => x.LabTests,
                x => x.Consultation_Requests,
                x => x.SelectedEmergencyProcedureses,
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
                    IsEmergencyPrescription = x.IsEmergencyPrescription,
                    EmergencyCaseId = x.EmergencyCaseId,
                    DepartmentId = x.DepartmentId,
                    SpecialistDoctorId = x.SpecialistDoctorId,
                    IsSpecialAdviceRequired = x.IsSpecialAdviceRequired,
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
                        MedicineId = i.MedicineId,
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
                    }).ToList(),
                    SelectedEmergencyProcedureses = x.SelectedEmergencyProcedureses.Select(lt => new EMRSystem.EmergencyProcedure.SelectedEmergencyProcedures
                    {
                        Id = lt.Id,
                        EmergencyProcedureId = lt.EmergencyProcedureId,
                        PrescriptionId = lt.PrescriptionId,
                    }).ToList(),
                    Consultation_Requests = x.Consultation_Requests == null ? null : new EMRSystem.Doctors.ConsultationRequests
                    {
                        Id = x.Consultation_Requests.Id,
                        PrescriptionId = x.Id,
                        RequestingDoctorId = x.Consultation_Requests.RequestingDoctor.Id,
                        RequestedSpecialistId = x.Consultation_Requests.RequestedSpecialist.Id,
                        Status = x.Consultation_Requests.Status,
                        Notes = x.Consultation_Requests.Notes,
                        AdviceResponse = x.Consultation_Requests.AdviceResponse,
                    },
                })
                .FirstOrDefaultAsync();
            if (details == null)
            {
                throw new EntityNotFoundException(typeof(Prescription), id);
            }

            var prescription = ObjectMapper.Map<CreateUpdatePrescriptionDto>(details);
            prescription.CreateUpdateConsultationRequests = ObjectMapper.Map<CreateUpdateConsultationRequestsDto>(details.Consultation_Requests);
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

        public async Task<ListResultDto<PrescriptionDto>> GetPrescriptionsByPatient(long patientId)
        {
            var query = Repository.GetAll()
                .Include(p => p.Patient)
                .Include(p => p.Doctor)
                .Include(p => p.LabTests)
                .Include(p => p.Items)
                .Where(p => p.Patient.Id == patientId)
                .OrderByDescending(p => p.IssueDate);

            var prescriptions = await query
                .Select(p => new PrescriptionDto
                {
                    Id = p.Id,
                    TenantId = p.TenantId,
                    Diagnosis = p.Diagnosis,
                    Notes = p.Notes,
                    IssueDate = p.IssueDate,
                    IsFollowUpRequired = p.IsFollowUpRequired,
                    Patient = new PatientDto
                    {
                        Id = p.Patient.Id,
                        FullName = p.Patient.FullName
                        // Add other needed PatientDto properties here
                    },
                    Doctor = new DoctorDto
                    {
                        Id = p.Doctor.Id,
                        FullName = p.Doctor.FullName
                        // Add other needed DoctorDto properties here
                    },
                    LabTestIds = p.LabTests.Select(lt => lt.LabReportsTypeId).ToList(),
                    PharmacistPrescription = p.Items.Select(x => new PharmacistPrescriptionItemWithUnitPriceDto
                    {
                        PrescriptionId = x.PrescriptionId,
                        MedicineId = x.MedicineId,
                        MedicineName = x.MedicineName,
                        Dosage = x.Dosage,
                        Frequency = x.Frequency,
                        Duration = x.Duration,
                        Instructions = x.Instructions,
                        Qty = x.Qty,
                        UnitPrice = _pharmacistInventoryRepository.Get(x.MedicineId).SellingPrice,
                        TotalPayableAmount = 0
                    }).ToList()

                    // Add other properties if needed
                }).ToListAsync();

            return new ListResultDto<PrescriptionDto>(prescriptions);
        }

        public async Task CreateUpdateCharges(CreateUpdatePrescriptionDto input, long? prescriptionId, bool isNewRecord)
        {
            if (input.IsEmergencyPrescription)
            {
                if (!isNewRecord)
                {
                    var getList = await _emergencyChargeEntriesRepository.GetAll().Where(x => x.PrescriptionId == prescriptionId).ToListAsync();
                    if (getList.Count > 0)
                    {
                        _emergencyChargeEntriesRepository.RemoveRange(getList);
                    }
                }
                // takke charge for emergency doctor
                {
                    var emergencyDoctor = await _doctorMasterRepository
                                    .FirstOrDefaultAsync(dm => dm.DoctorId == input.DoctorId);
                    var chargeEntry = new EMRSystem.EmergencyChargeEntries.EmergencyChargeEntry
                    {
                        PatientId = input.PatientId,
                        ChargeType = ChargeType.Other,
                        Description = $"Emergency Doctor Charge",
                        Amount = emergencyDoctor.Fee > 0 ? emergencyDoctor.Fee : 0,
                        EmergencyCaseId = input.EmergencyCaseId,
                        PrescriptionId = prescriptionId,
                    };
                    await _emergencyChargeEntriesRepository.InsertAsync(chargeEntry);
                }

                // take charge for procedure if selected
                {
                    if (input.EmergencyProcedures.Count > 0)
                    {
                        foreach (var itm in input.EmergencyProcedures)
                        {
                            var procedure = await _emergencyProcedureRepository.FirstOrDefaultAsync(dm => dm.Id == itm.EmergencyProcedureId);
                            var chargeEntry = new EMRSystem.EmergencyChargeEntries.EmergencyChargeEntry
                            {
                                PatientId = input.PatientId,
                                ChargeType = ChargeType.Procedure,
                                Description = $"Emergency Procedure Charge",
                                Amount = procedure.DefaultCharge > 0 ? procedure.DefaultCharge : 0,
                                EmergencyCaseId = input.EmergencyCaseId,
                                PrescriptionId = prescriptionId,
                            };
                            await _emergencyChargeEntriesRepository.InsertRangeAsync(chargeEntry);
                        }

                    }
                }

                // take charge for advice if selected
                {
                    if (input.IsSpecialAdviceRequired && input.SpecialistDoctorId != null)
                    {
                        var specialDoctor = await _doctorMasterRepository.FirstOrDefaultAsync(dm => dm.DoctorId == input.SpecialistDoctorId);
                        var chargeEntry = new EMRSystem.EmergencyChargeEntries.EmergencyChargeEntry
                        {
                            PatientId = input.PatientId,
                            ChargeType = ChargeType.Other,
                            Description = $"Emergency Special Doctor Advice Charge",
                            Amount = specialDoctor.Fee > 0 ? specialDoctor.Fee : 0,
                            EmergencyCaseId = input.EmergencyCaseId,
                            PrescriptionId = prescriptionId,
                        };
                        await _emergencyChargeEntriesRepository.InsertAsync(chargeEntry);
                    }
                }
            }
        }

        public async Task CreateConsultationRequest(CreateUpdateConsultationRequestsDto requestsDto)
        {
            var mappedEntity = ObjectMapper.Map<EMRSystem.Doctors.ConsultationRequests>(requestsDto);
            if (requestsDto.Id > 0)
            {
                await _consultationRequestsRepository.UpdateAsync(mappedEntity);
            }
            else
            {
                await _consultationRequestsRepository.InsertAsync(mappedEntity);
            }
        }

        private (int value, string unit) ParseDuration(string duration)
        {
            if (string.IsNullOrWhiteSpace(duration))
                return (0, "Days");

            var parts = duration.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length < 2) return (0, "Days");

            return (int.TryParse(parts[0], out int val) ? val : 0, parts[1]);
        }

        private int CalculateQty(string frequency, string duration)
        {
            var (value, unit) = ParseDuration(duration);

            // Convert duration to days
            int totalDays = unit switch
            {
                "Days" => value,
                "Weeks" => value * 7,
                "Months" => value * 30,
                _ => value
            };

            // Map frequency to times per day
            int timesPerDay = frequency switch
            {
                "Once a day" => 1,
                "Twice a day" => 2,
                "Three times a day" => 3,
                "Four times a day" => 4,
                "Every 6 hours" => 4,
                "Every 8 hours" => 3,
                "Every 12 hours" => 2,
                "As needed" => 0, // PRN can't be predicted
                _ => 0
            };

            var res = totalDays * timesPerDay;
            return res;
        }

        public async Task<decimal> GetGrandTotal(long _id)
        {
            var list = Repository.GetAllIncluding(x => x.Items).FirstOrDefault(x => x.Id == _id);
            if (list != null && list.Items != null)
            {
                return list.Items.Sum(x => x.Qty * (_pharmacistInventoryRepository.Get(x.MedicineId).SellingPrice));
            }
            else
            {
                return 0;
            }
        }
    }
}