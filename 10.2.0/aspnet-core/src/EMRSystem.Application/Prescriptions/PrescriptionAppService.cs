using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Domain.Entities;
using Abp.Domain.Repositories;
using Abp.EntityFrameworkCore.Repositories;
using Abp.Extensions;
using Abp.Linq.Extensions;
using Abp.UI;
using EMRSystem.Appointments;
using EMRSystem.Authorization.Users;
using EMRSystem.Doctor;
using EMRSystem.Doctor.Dto;
using EMRSystem.Doctors;
using EMRSystem.EmergencyProcedure;
using EMRSystem.EmergencyProcedure.Dto;
using EMRSystem.IpdChargeEntry;
using EMRSystem.LabReports;
using EMRSystem.Medicines;
using EMRSystem.NumberingService;
using EMRSystem.Patients;
using EMRSystem.Patients.Dto;
using EMRSystem.Pharmacists;
using EMRSystem.Prescriptions.Dto;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;



namespace EMRSystem.Prescriptions
{
    public class PrescriptionAppService : AsyncCrudAppService<Prescription, PrescriptionDto, long, PagedPrescriptionResultRequestDto, CreateUpdatePrescriptionDto, CreateUpdatePrescriptionDto>,
   IPrescriptionAppService
    {
        private readonly IDoctorAppService _doctorAppService;
        private readonly IRepository<Appointment, long> _appointmentRepository;
        private readonly IRepository<PrescriptionItem, long> _prescriptionItemRepository;
        private readonly IRepository<Patient, long> _patientRepository;
        private readonly IRepository<MedicineStock, long> _medicineStockRepository;
        private readonly IRepository<EMRSystem.LabReports.PrescriptionLabTest, long> _prescriptionLabTestRepository;
        private readonly IRepository<EMRSystem.EmergencyChargeEntries.EmergencyChargeEntry, long> _emergencyChargeEntriesRepository;
        private readonly IRepository<EMRSystem.DoctorMaster.DoctorMaster, long> _doctorMasterRepository;
        private readonly IRepository<EMRSystem.EmergencyProcedure.EmergencyProcedure, long> _emergencyProcedureRepository;
        private readonly IRepository<EMRSystem.IpdChargeEntry.IpdChargeEntry, long> _ipdChargeEntryRepository;
        private readonly IRepository<EMRSystem.Admission.Admission, long> _admissionRepository;
        private readonly INumberingService _numberingService;
        private readonly IRepository<EMRSystem.Doctors.ConsultationRequests, long> _consultationRequestsRepository;
        private readonly IRepository<EMRSystem.Pharmacists.PharmacistPrescriptions, long> _pharmacistPrescriptionsRepository;
        private readonly IRepository<EMRSystem.Pharmacists.PharmacistInventory, long> _pharmacistInventoryRepository;

        private readonly UserManager _userManager;
        public PrescriptionAppService(
            IRepository<Prescription, long> repository,
            IRepository<EMRSystem.Admission.Admission, long> admissionRepository,
            IRepository<EMRSystem.IpdChargeEntry.IpdChargeEntry, long> ipdChargeEntryRepository,
            IRepository<PrescriptionItem, long> prescriptionItemRepository,
            IRepository<MedicineStock, long> medicineStockRepository,
            INumberingService numberingService,
            IDoctorAppService doctorAppService, UserManager userManager,
            IRepository<Appointment, long> appointmentRepository,
            IRepository<Patient, long> patientRepository,
            IRepository<EMRSystem.LabReports.PrescriptionLabTest, long> prescriptionLabTestRepository,
            IRepository<EmergencyChargeEntries.EmergencyChargeEntry, long> emergencyChargeEntriesRepository,
            IRepository<DoctorMaster.DoctorMaster, long> doctorMasterRepository,
            IRepository<EmergencyProcedure.EmergencyProcedure, long> emergencyProcedureRepository,
            IRepository<Doctors.ConsultationRequests, long> consultationRequestsRepository,
            IRepository<EMRSystem.Pharmacists.PharmacistPrescriptions, long> pharmacistPrescriptionsRepository,
            IRepository<Pharmacists.PharmacistInventory, long> pharmacistInventoryRepository
            ) : base(repository)
        {
            _doctorAppService = doctorAppService;
            _userManager = userManager;
            _numberingService = numberingService;
            _prescriptionItemRepository = prescriptionItemRepository;
            _appointmentRepository = appointmentRepository;
            _patientRepository = patientRepository;
            _prescriptionLabTestRepository = prescriptionLabTestRepository;
            _emergencyChargeEntriesRepository = emergencyChargeEntriesRepository;
            _doctorMasterRepository = doctorMasterRepository;
            _emergencyProcedureRepository = emergencyProcedureRepository;
            _ipdChargeEntryRepository = ipdChargeEntryRepository;
            _admissionRepository = admissionRepository;
            _consultationRequestsRepository = consultationRequestsRepository;
            _pharmacistPrescriptionsRepository = pharmacistPrescriptionsRepository;
            _pharmacistInventoryRepository = pharmacistInventoryRepository;
            _medicineStockRepository = medicineStockRepository;
        }
        protected override IQueryable<Prescription> CreateFilteredQuery(PagedPrescriptionResultRequestDto input)
        {
            try
            {

                var userId = AbpSession.UserId;
                var doctor = _doctorAppService.GetDoctorDetailsByAbpUserID(userId.Value);
                var fromDateLocal = input.FromDate?.ToLocalTime();
                var toDateLocal = input.ToDate?.ToLocalTime();




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
                    .WhereIf(fromDateLocal.HasValue, x => x.IssueDate >= fromDateLocal.Value)
                    .WhereIf(toDateLocal.HasValue, x => x.IssueDate <= toDateLocal.Value)
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
                            PharmacistPrescriptionId = i.PharmacistPrescriptionId
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
            bool isAdmitted = false;
            // Fetch patient to check admission
            if (input.PatientId.HasValue)
            {
                var patient = await _patientRepository.GetAsync(input.PatientId.Value);
                isAdmitted = patient.IsAdmitted;
            }

            //// Map Items
            //prescription.Items = input.Items
            //    .Select(item =>
            //    {
            //        item.UnitPrice = _pharmacistInventoryRepository.Get(item.MedicineId).SellingPrice;
            //        return ObjectMapper.Map<PrescriptionItem>(item);
            //    }).ToList();

            prescription.Items = input.Items
                .Select(item =>
                {
                    var stock = _medicineStockRepository.GetAll()
                        .Where(ms => ms.MedicineMasterId == item.MedicineId
                                     && !ms.IsExpire
                                     && ms.ExpiryDate >= DateTime.Today)
                        .OrderBy(ms => ms.ExpiryDate) // सबसे नज़दीकी expiry वाला लेगा
                        .FirstOrDefault();

                    if (stock == null)
                    {
                        throw new UserFriendlyException($"No valid stock found for medicine ID {item.MedicineId}");
                    }

                    item.UnitPrice = stock.SellingPrice;

                    return ObjectMapper.Map<PrescriptionItem>(item);
                })
                .ToList();

            if (input.EmergencyProcedures != null && input.EmergencyProcedures.Any())
            {
                prescription.SelectedEmergencyProcedureses = input.EmergencyProcedures
                    .Select(itm =>
                    {
                        var mapped = ObjectMapper.Map<SelectedEmergencyProcedures>(itm);

                        mapped.IsPaid = (isAdmitted || input.IsEmergencyPrescription);
                        mapped.Status = EmergencyProcedureStatus.Pending;

                        return mapped;
                    }).ToList();
            }



            // Save prescription (to get Id)
            await Repository.InsertAsync(prescription);
            await CurrentUnitOfWork.SaveChangesAsync(); // Needed to get prescription.Id



            if (isAdmitted)
            {
                var admission = await _admissionRepository.FirstOrDefaultAsync(a =>
                    a.PatientId == input.PatientId && !a.IsDischarged);

                if (admission == null)
                {
                    throw new UserFriendlyException("No active admission found for this patient.");
                }
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
                // Add Doctor Consultation Fee to IpdChargeEntry
                if (input.DoctorId.HasValue)
                {
                    var doctor = await _doctorMasterRepository
                        .GetAllIncluding(dm => dm.Doctor) // include related doctor entity
                        .FirstOrDefaultAsync(dm => dm.DoctorId == input.DoctorId);

                    if (doctor != null && doctor.Fee > 0)
                    {
                        var ipdDoctorCharge = new EMRSystem.IpdChargeEntry.IpdChargeEntry
                        {
                            TenantId = input.TenantId,
                            AdmissionId = admission.Id,
                            PatientId = input.PatientId.Value,
                            ChargeType = ChargeType.ConsultationFee,
                            Description = $"Consultation Fee - Dr. {doctor.Doctor.FullName}",
                            Quantity =1,
                            Amount = doctor.Fee,
                            ReferenceId = input.DoctorId,
                            PrescriptionId = prescription.Id
                        };

                        await _ipdChargeEntryRepository.InsertAsync(ipdDoctorCharge);
                    }
                }


                // Add Procedure Charges to IpdChargeEntry
                if (input.EmergencyProcedures != null && input.EmergencyProcedures.Count > 0)
                {
                    foreach (var itm in input.EmergencyProcedures)
                    {
                        var procedure = await _emergencyProcedureRepository
                            .FirstOrDefaultAsync(p => p.Id == itm.EmergencyProcedureId);

                        if (procedure != null && procedure.DefaultCharge > 0)
                        {
                            var ipdProcedureCharge = new EMRSystem.IpdChargeEntry.IpdChargeEntry
                            {
                                TenantId = input.TenantId,
                                AdmissionId = admission.Id,
                                PatientId = input.PatientId.Value,
                                ChargeType = ChargeType.Procedure,
                                Quantity =1,
                                Description = $"Procedure Charge - {procedure.Name}",
                                Amount = procedure.DefaultCharge,
                                ReferenceId = procedure.Id,
                                PrescriptionId = prescription.Id
                            };

                            await _ipdChargeEntryRepository.InsertAsync(ipdProcedureCharge);
                        }
                    }
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


            if (input.Items.Count > 0)
            {
                // create Pharmacist Prescriptions
                var pharmacistPrescriptionsdto = new EMRSystem.Pharmacists.PharmacistPrescriptions();
                pharmacistPrescriptionsdto.TenantId = input.TenantId;
                pharmacistPrescriptionsdto.PrescriptionId = prescription.Id;
                pharmacistPrescriptionsdto.IssueDate = DateTime.Now;
                pharmacistPrescriptionsdto.CollectionStatus = CollectionStatus.NotPickedUp;
                if (isAdmitted)
                {
                    pharmacistPrescriptionsdto.IsPaid = true;
                    pharmacistPrescriptionsdto.ReceiptNumber = await GenerateReceiptNoAsync(input.TenantId);
                }
                else if (input.IsEmergencyPrescription)
                {
                    pharmacistPrescriptionsdto.IsPaid = true;
                    pharmacistPrescriptionsdto.ReceiptNumber = await GenerateReceiptNoAsync(input.TenantId);

                }
                else
                {
                    pharmacistPrescriptionsdto.IsPaid = false;
                }
                pharmacistPrescriptionsdto.GrandTotal = 0;//await GetGrandTotal(prescription.Id);
                var res = await _pharmacistPrescriptionsRepository.InsertAndGetIdAsync(pharmacistPrescriptionsdto);
            }
            await CurrentUnitOfWork.SaveChangesAsync();
        }
        public async Task<string> GenerateReceiptNoAsync(int tenantId)
        {
            return await _numberingService.GenerateReceiptNumberAsync(
                _pharmacistPrescriptionsRepository,
                "MED-REC",
                tenantId,
                "ReceiptNumber"
            );
        }
        public async Task UpdatePrescriptionWithItemAsync(CreateUpdatePrescriptionDto input)
        {
            // First get the existing prescription with its items
            var existingPrescription = await Repository.GetAllIncluding(
                p => p.Items.Where(i => i.PharmacistPrescriptionId == null),
                p => p.LabTests,
                p => p.SelectedEmergencyProcedureses
            ).FirstOrDefaultAsync(p => p.Id == input.Id);

            if (existingPrescription == null)
            {
                throw new UserFriendlyException("Prescription not found");
            }
            var admission = await _admissionRepository.FirstOrDefaultAsync(a =>
    a.PatientId == input.PatientId && !a.IsDischarged);

            bool isPatientAdmitted = admission != null;
            bool isPaid = input.IsEmergencyPrescription || isPatientAdmitted;
            bool isPrescribed = !input.IsEmergencyPrescription;

            // Map the input to the existing prescription
            ObjectMapper.Map(input, existingPrescription);

            // Handle Items update
            var existingItems = existingPrescription.Items.ToList();

            // Update existing items or add new ones
            foreach (var inputItem in input.Items)
            {
                // Stock से UnitPrice निकालना
                var stock = await _medicineStockRepository.GetAll()
                    .Where(ms => ms.MedicineMasterId == inputItem.MedicineId
                                 && !ms.IsExpire
                                 && ms.ExpiryDate >= DateTime.Today)
                    .OrderBy(ms => ms.ExpiryDate)
                    .FirstOrDefaultAsync();

                inputItem.UnitPrice = stock != null ? stock.SellingPrice : 0;

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
            var itemsToRemove = existingItems
                .Where(ei => !input.Items.Any(ii => ii.Id == ei.Id))
                .ToList();

            foreach (var item in itemsToRemove)
            {
                await _prescriptionItemRepository.DeleteAsync(item);
            }

            // Handle LabTests update
            var existingLabTests = existingPrescription.LabTests.ToList();

            // remove those which are no longer in input
            var labTestsToRemove = existingLabTests
                .Where(ei => !input.LabTestIds.Contains(ei.LabReportsTypeId)) // ✅ no cast
                .ToList();

            if (labTestsToRemove.Any())
            {
                foreach (var lab in labTestsToRemove)
                {
                    await _prescriptionLabTestRepository.DeleteAsync(lab.Id); // ✅ direct delete
                }
            }

            // add new ones
            foreach (var labTestId in input.LabTestIds)
            {
                if (!existingLabTests.Any(l => l.LabReportsTypeId == labTestId)) // ✅ no cast
                {
                    existingPrescription.LabTests.Add(new EMRSystem.LabReports.PrescriptionLabTest
                    {
                        LabReportsTypeId = labTestId, // ✅ no cast
                        PrescriptionId = input.Id,
                        IsEmergencyPrescription = input.IsEmergencyPrescription,
                        EmergencyCaseId = input.EmergencyCaseId,
                        IsPrescribed = isPrescribed,
                        IsPaid = isPaid,
                        PatientId = input.PatientId,
                    });
                }
            }




            // Handle procedure update
            // Handle procedure update
            var existingProcedure = existingPrescription.SelectedEmergencyProcedureses.ToList();

            // Update existing items or add new ones
            if (input.EmergencyProcedures != null && input.EmergencyProcedures.Any())
            {
                foreach (var inputItem2 in input.EmergencyProcedures)
                {
                    var existingItem2 = existingProcedure
                        .FirstOrDefault(i => i.EmergencyProcedureId == inputItem2.EmergencyProcedureId);

                    if (existingItem2 != null)
                    {
                        ObjectMapper.Map(inputItem2, existingItem2);

                        // ✅ agar emergency prescription hai to IsPaid = true force kar do
                        if (input.IsEmergencyPrescription)
                        {
                            existingItem2.IsPaid = true;
                        }
                    }
                    else
                    {
                        var newItem2 = ObjectMapper.Map<EMRSystem.EmergencyProcedure.SelectedEmergencyProcedures>(inputItem2);

                        // ✅ agar emergency prescription hai to IsPaid = true force kar do
                        if (input.IsEmergencyPrescription)
                        {
                            newItem2.IsPaid = true;
                        }

                        existingPrescription.SelectedEmergencyProcedureses.Add(newItem2);
                    }
                }
            }

            // Remove items that are no longer in the input
            var itemsToRemove2 = existingProcedure
                .Where(ei => !input.EmergencyProcedures.Any(ii => ii.EmergencyProcedureId == ei.EmergencyProcedureId))
                .ToList();

            foreach (var item2 in itemsToRemove2)
            {
                existingPrescription.SelectedEmergencyProcedureses.Remove(item2);
            }


            await Repository.UpdateAsync(existingPrescription);
            await CurrentUnitOfWork.SaveChangesAsync();
            if (!input.IsEmergencyPrescription)
            {


                if (admission != null)
                {
                    // get old procedure charges for this prescription
                    var existingIpdCharges = await _ipdChargeEntryRepository
                        .GetAll()
                        .Where(c => c.PrescriptionId == input.Id && c.ChargeType == ChargeType.Procedure)
                        .ToListAsync();

                    // remove those which are no longer in input
                    var toRemove = existingIpdCharges
                        .Where(c => !input.EmergencyProcedures.Any(p => p.EmergencyProcedureId == c.ReferenceId))
                        .ToList();

                    if (toRemove.Any())
                    {
                        _ipdChargeEntryRepository.RemoveRange(toRemove);
                    }

                    // add new charges if any
                    foreach (var proc in input.EmergencyProcedures)
                    {
                        if (!existingIpdCharges.Any(c => c.ReferenceId == proc.EmergencyProcedureId))
                        {
                            var procedure = await _emergencyProcedureRepository
                                .FirstOrDefaultAsync(p => p.Id == proc.EmergencyProcedureId);

                            if (procedure != null && procedure.DefaultCharge > 0)
                            {
                                var ipdProcedureCharge = new EMRSystem.IpdChargeEntry.IpdChargeEntry
                                {
                                    TenantId = input.TenantId,
                                    AdmissionId = admission.Id,
                                    PatientId = input.PatientId.Value,
                                    ChargeType = ChargeType.Procedure,
                                    Description = $"Procedure Charge:- {procedure.Name}",
                                    Quantity =1,
                                    Amount = procedure.DefaultCharge,
                                    ReferenceId = procedure.Id,
                                    PrescriptionId = input.Id
                                };

                                await _ipdChargeEntryRepository.InsertAsync(ipdProcedureCharge);
                            }
                        }
                    }
                }
            }
            if (input.IsSpecialAdviceRequired)
            {
                input.CreateUpdateConsultationRequests.PrescriptionId = existingPrescription.Id;
                await CreateConsultationRequest(input.CreateUpdateConsultationRequests);
            }

            if (input.Items.Count <= 0)
            {
                var res = await _pharmacistPrescriptionsRepository.GetAll().FirstOrDefaultAsync(x => x.PrescriptionId == input.Id);
                await _pharmacistPrescriptionsRepository.DeleteAsync(res);
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
                        MedicineFormId = i.MedicineFormId,
                        Duration = i.Duration,
                        Instructions = i.Instructions,
                        PharmacistPrescriptionId = i.PharmacistPrescriptionId
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
            prescription.LabTestIds = details.LabTests.Select(lt => lt.LabReportsTypeId).ToList();
            prescription.CreateUpdateConsultationRequests = ObjectMapper.Map<CreateUpdateConsultationRequestsDto>(details.Consultation_Requests);

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
                .Include(p => p.SelectedEmergencyProcedureses)
                    .ThenInclude(sep => sep.EmergencyProcedures)
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
                    }),
                    Procedures = p.SelectedEmergencyProcedureses.Select(sep => sep.EmergencyProcedures.Name)
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
                }).ToList(),
                ProcedureNames = prescription.Procedures.ToList()
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
                .Include(p => p.SelectedEmergencyProcedureses)
                    .ThenInclude(sep => sep.EmergencyProcedures)
                .Where(p => p.Patient.Id == patientId)
                .OrderByDescending(p => p.IssueDate);

            // पहले पूरे prescriptions entities लाओ
            var prescriptions = await query.ToListAsync();

            // अब C# side mapping करो
            var result = prescriptions.Select(p => new PrescriptionDto
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
                },
                Doctor = new DoctorDto
                {
                    Id = p.Doctor.Id,
                    FullName = p.Doctor.FullName
                },

                LabTestIds = p.LabTests.Select(lt => lt.LabReportsTypeId).ToList(),

                PharmacistPrescription = p.Items.Select(x =>
                {
                    // memory में stock check करो
                    var stock = _medicineStockRepository.GetAll()
                        .Where(ms => ms.MedicineMasterId == x.MedicineId
                                     && !ms.IsExpire
                                     && ms.ExpiryDate >= DateTime.Today)
                        .OrderBy(ms => ms.ExpiryDate)
                        .FirstOrDefault();

                    var unitPrice = stock != null ? stock.SellingPrice : 0;

                    return new PharmacistPrescriptionItemWithUnitPriceDto
                    {
                        PrescriptionId = x.PrescriptionId,
                        MedicineId = x.MedicineId,
                        MedicineName = x.MedicineName,
                        Dosage = x.Dosage,
                        Frequency = x.Frequency,
                        Duration = x.Duration,
                        Instructions = x.Instructions,
                        Qty = x.Qty,
                        UnitPrice = unitPrice,
                        IsPrescribe = x.IsPrescribe,
                        TotalPayableAmount = x.Qty * unitPrice,
                        MedicineFormId = x.MedicineFormId,
                        PharmacistPrescriptionId = x.PharmacistPrescriptionId,
                    };
                }).ToList(),

                Procedures = p.SelectedEmergencyProcedureses.Select(sep => new SelectedEmergencyProceduresDto
                {
                    Id = sep.Id,
                    TenantId = sep.TenantId,
                    IsPaid = sep.IsPaid,
                    Status = sep.Status,
                    EmergencyProcedures = new EmergencyProcedureDto
                    {
                        Id = sep.EmergencyProcedures.Id,
                        TenantId = sep.EmergencyProcedures.TenantId,
                        Name = sep.EmergencyProcedures.Name,
                        Category = sep.EmergencyProcedures.Category,
                        DefaultCharge = sep.EmergencyProcedures.DefaultCharge,
                        IsActive = sep.EmergencyProcedures.IsActive
                    },
                    ProcedureName = sep.EmergencyProcedures.Name,
                    PatientName = p.Patient.FullName
                }).ToList()

            }).ToList();

            return new ListResultDto<PrescriptionDto>(result);
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
                                     .GetAllIncluding(dm => dm.Doctor)
                                    .FirstOrDefaultAsync(dm => dm.DoctorId == input.DoctorId);
                    var chargeEntry = new EMRSystem.EmergencyChargeEntries.EmergencyChargeEntry
                    {
                        PatientId = input.PatientId,
                        ChargeType = ChargeType.Other,
                        Description = $"Emergency Doctor Charge - Dr. {emergencyDoctor.Doctor.FullName}",
                        Quantity =1,
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
                                Description = $"Emergency Procedure Charge - {procedure.Name}",
                                Quantity =1,
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
                        var specialDoctor = await _doctorMasterRepository
                            .GetAllIncluding(dm => dm.Doctor)
                            .FirstOrDefaultAsync(dm => dm.DoctorId == input.SpecialistDoctorId);
                        var chargeEntry = new EMRSystem.EmergencyChargeEntries.EmergencyChargeEntry
                        {
                            PatientId = input.PatientId,
                            ChargeType = ChargeType.Other,
                            Description = $"Emergency Special Doctor Advice Charge - Dr. {specialDoctor.Doctor.FullName}",
                            Quantity = 1,
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

        public override async Task DeleteAsync(EntityDto<long> input)
        {
            try
            {
                var entity = await Repository.GetAll()
                    .Include(x => x.Consultation_Requests)
                    .Include(x => x.PharmacistPrescriptions)
                    .Include(x => x.IpdChargeEntries)
                    .Include(x => x.EmergencyChargeEntries)
                    .Include(x => x.SelectedEmergencyProcedureses)
                    .Include(x => x.LabTests).ThenInclude(x => x.LabReportResultItems)
                    .Include(x => x.Items)
               .Where(x => x.Id == input.Id).FirstOrDefaultAsync();
                if (entity != null)
                {
                    await Repository.DeleteAsync(entity);
                    await CurrentUnitOfWork.SaveChangesAsync();
                }
            }
            catch (Exception dbEx)
            {
                throw new UserFriendlyException(
                    "Delete failed",
                    dbEx.GetBaseException().Message
                );
            }
        }
    }
}