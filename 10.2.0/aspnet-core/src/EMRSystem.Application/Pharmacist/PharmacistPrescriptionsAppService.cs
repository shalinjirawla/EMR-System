using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Domain.Repositories;
using Abp.EntityFrameworkCore.Repositories;
using Abp.Linq.Extensions;
using Abp.UI;
using EMRSystem.Admission;
using EMRSystem.Appointments;
using EMRSystem.Doctors;
using EMRSystem.EmergencyChargeEntries;
using EMRSystem.EmergencyProcedure;
using EMRSystem.Invoices;
using EMRSystem.IpdChargeEntry;
using EMRSystem.LabReport.Dto;
using EMRSystem.LabReports;
using EMRSystem.MedicineOrder;
using EMRSystem.Medicines;
using EMRSystem.MultiTenancy;
using EMRSystem.NumberingService;
using EMRSystem.Patients;
using EMRSystem.Pharmacist.Dto;
using EMRSystem.Pharmacists;
using EMRSystem.Prescriptions;
using EMRSystem.Prescriptions.Dto;
using EMRSystem.TempStripeData;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Stripe.Checkout;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using static Castle.MicroKernel.ModelBuilder.Descriptors.InterceptorDescriptor;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace EMRSystem.Pharmacist
{

    public class PharmacistPrescriptionsAppService : AsyncCrudAppService<
        EMRSystem.Pharmacists.PharmacistPrescriptions, 
        PharmacistPrescriptionsDto,
        long,
        PagedPharmacistPrescriptionDto,
        CreateUpdatePharmacistPrescriptionsDto,
        CreateUpdatePharmacistPrescriptionsDto>,
     IPharmacistPrescriptionsAppService
    {

        private readonly IRepository<EMRSystem.Pharmacists.PharmacistInventory, long> _pharmacistInventoryRepository;
        private readonly IRepository<EMRSystem.Prescriptions.PrescriptionItem, long> _prescriptionItemRepository;
        private readonly TenantManager _tenantManager;
        private readonly INumberingService _numberingService;
        private readonly IRepository<EmergencyChargeEntry, long> _emergencyChargeRepository;
        private readonly IRepository<EMRSystem.IpdChargeEntry.IpdChargeEntry, long> _ipdChargeRepository;
        private readonly ITempStripeDataService _tempStripeDataService;
        private readonly IRepository<MedicineStock, long> _medicineStockRepository;

        public PharmacistPrescriptionsAppService(
            IRepository<EMRSystem.Pharmacists.PharmacistPrescriptions, long> repository,
            INumberingService numberingService,
            IRepository<MedicineStock, long> medicineStockRepository,
            IRepository<EmergencyChargeEntry, long> emergencyChargeRepository,
            IRepository<EMRSystem.IpdChargeEntry.IpdChargeEntry, long> ipdChargeRepository,

        ITempStripeDataService tempStripeDataService,
            IRepository<Pharmacists.PharmacistInventory, long> pharmacistInventoryRepository, TenantManager tenantManager,
            IRepository<PrescriptionItem, long> prescriptionItemRepository) : base(repository)
        {
            _pharmacistInventoryRepository = pharmacistInventoryRepository;
            _tenantManager = tenantManager;
            _emergencyChargeRepository = emergencyChargeRepository;
            _ipdChargeRepository = ipdChargeRepository;
            _numberingService = numberingService;
            _tempStripeDataService = tempStripeDataService;
            _medicineStockRepository = medicineStockRepository;
            _prescriptionItemRepository = prescriptionItemRepository;
        }

        [HttpGet]
        public async Task<PagedResultDto<PharmacistPrescriptionsDto>> GetPrescriptionFulfillment(PagedPharmacistPrescriptionDto input)
        {
            try
            {
                var query = Repository.GetAllIncluding(
                        x => x.Prescriptions,
                        x => x.Prescriptions.Patient,
                        x => x.Prescriptions.Doctor,
                        x => x.Prescriptions.Items
                    )
                    .Where(x => x.Prescriptions != null && x.Prescriptions.Items != null)
                    .WhereIf(!string.IsNullOrWhiteSpace(input.Keyword),
                        x =>
                            x.Prescriptions.Patient.FullName.ToLower().Contains(input.Keyword.ToLower()) ||
                            x.Prescriptions.Doctor.FullName.ToLower().Contains(input.Keyword.ToLower())
                    );

                var totalCount = await query.CountAsync();

                // ✅ Use Dynamic LINQ for string sorting
                query = !string.IsNullOrWhiteSpace(input.Sorting)
                    ? query.OrderBy(input.Sorting)
                    : query.OrderByDescending(x => x.Id);

                var list = await query
                    .Skip(input.SkipCount)
                    .Take(input.MaxResultCount)
                    .ToListAsync();

                var mappedItems = ObjectMapper.Map<List<PharmacistPrescriptionsDto>>(list);

                foreach (var prescription in mappedItems)
                {
                    if (prescription?.prescriptionItems != null)
                    {
                        foreach (var item in prescription.prescriptionItems)
                        {
                            var stock = await _medicineStockRepository.GetAll()
                                .Where(ms => ms.MedicineMasterId == item.MedicineId
                                             && !ms.IsExpire
                                             && ms.ExpiryDate >= DateTime.Today)
                                .OrderBy(ms => ms.ExpiryDate)
                                .FirstOrDefaultAsync();

                            item.UnitPrice = stock != null ? stock.SellingPrice : 0;
                        }
                    }
                }

                return new PagedResultDto<PharmacistPrescriptionsDto>(totalCount, mappedItems);
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException("Error in GetPrescriptionFulfillment", ex);
            }
        }
        public async Task<string> GenerateReceiptNoAsync(int tenantId)
        {
            return await _numberingService.GenerateReceiptNumberAsync(
                Repository,
                "MED-REC",
                tenantId,
                "ReceiptNumber"
            );
        }
        [HttpPost]
        public async Task<string> HandlePharmacistPrescriptionPayment(CreatePharmacistPrescriptionsWithItemDto withItemDto)
        {
            if (withItemDto.pharmacistPrescriptionsDto.PaymentMethod == PaymentMethod.Cash)
            {
                // Cash → Direct Create
                await CreatePharmacistPrescriptionsWithItem(withItemDto);
                return "Cash payment successful. Prescription created.";
            }
            else if (withItemDto.pharmacistPrescriptionsDto.PaymentMethod == PaymentMethod.Card)
            {
                var tempDataId = Guid.NewGuid().ToString();
                await _tempStripeDataService.StoreData(tempDataId, withItemDto);
                // Card → Create Stripe Checkout Session
                var options = new SessionCreateOptions
                {
                    PaymentMethodTypes = new List<string> { "card" },
                    LineItems = withItemDto.pharmacistPrescriptionsListOfItem.Select(i => new SessionLineItemOptions
                    {
                        PriceData = new SessionLineItemPriceDataOptions
                        {
                            Currency = "inr",
                            UnitAmountDecimal = i.UnitPrice * 100,
                            ProductData = new SessionLineItemPriceDataProductDataOptions
                            {
                                Name = i.MedicineName
                            }
                        },
                        Quantity = i.Qty
                    }).ToList(),
                    Mode = "payment",
                    SuccessUrl = "http://localhost:4200/app/pharmacist/pharmacist-prescriptions",
                    CancelUrl = "http://localhost:4200/app/pharmacist/pharmacist-prescriptions",
                    Metadata = new Dictionary<string, string>
            {
                { "purpose", "pharmacistPrescription" },
                { "tempDataId", tempDataId }, // ✅ Only store reference ID
                { "tenantId", withItemDto.pharmacistPrescriptionsDto.TenantId.ToString() }
            }
                };

                var service = new SessionService();
                var session = service.Create(options);
                return session.Url;
            }

            throw new UserFriendlyException("Invalid payment method");
        }
        [HttpPost]
        public async Task CreatePharmacistPrescriptionsWithItem(CreatePharmacistPrescriptionsWithItemDto withItemDto)
        {
            try
            {
                var prescription = ObjectMapper.Map<PharmacistPrescriptions>(withItemDto.pharmacistPrescriptionsDto);
                prescription.IsPaid = true;
                long? pharmacistPrescriptionId = null;

                if (withItemDto.pharmacistPrescriptionsDto.Id <= 0)
                {
                    var isAlreadyExist = await Repository.GetAll()
                        .FirstOrDefaultAsync(x => x.PrescriptionId == prescription.PrescriptionId);

                    if (isAlreadyExist != null)
                    {
                        isAlreadyExist.IsPaid = true;
                        isAlreadyExist.GrandTotal = prescription.GrandTotal;
                        isAlreadyExist.PickedUpByPatient = prescription.PickedUpByPatient;
                        isAlreadyExist.CollectionStatus = CollectionStatus.PickedUp;
                        isAlreadyExist.PharmacyNotes = prescription.PharmacyNotes;
                        isAlreadyExist.PaymentMethod = prescription.PaymentMethod;
                        isAlreadyExist.PaymentIntentId = prescription.PaymentIntentId;
                        isAlreadyExist.ReceiptNumber = await GenerateReceiptNoAsync(prescription.TenantId);

                        await Repository.UpdateAsync(isAlreadyExist);
                        pharmacistPrescriptionId = isAlreadyExist.Id;

                        var allItems = await _prescriptionItemRepository.GetAll()
                            .Where(x => x.PrescriptionId == prescription.PrescriptionId || x.PharmacistPrescriptionId == pharmacistPrescriptionId)
                            .ToListAsync();

                        foreach (var dto in withItemDto.pharmacistPrescriptionsListOfItem)
                        {
                            var existing = allItems.FirstOrDefault(x =>
                                x.MedicineId == dto.MedicineId && (
                                    (x.PrescriptionId != null && x.PrescriptionId == dto.PrescriptionId) ||
                                    (x.PrescriptionId == null && x.PharmacistPrescriptionId != null && x.PharmacistPrescriptionId == pharmacistPrescriptionId)
                                ));

                            if (existing != null)
                            {
                                
                                int oldQty = existing.Qty;
                                int newQty = dto.Qty;
                                int diff = newQty - oldQty;

                                if (diff != 0)
                                    await AdjustStockAsync(existing.MedicineId,dto.BatchId, diff);

                                existing.Qty = newQty;
                                existing.Dosage = dto.Dosage;
                                existing.Frequency = dto.Frequency;
                                existing.Duration = dto.Duration;
                                existing.Instructions = dto.Instructions;
                                existing.IsPrescribe = true;
                                existing.PharmacistPrescriptionId = pharmacistPrescriptionId;

                                await _prescriptionItemRepository.UpdateAsync(existing);
                            }
                            else
                            {
                                if (dto.PrescriptionId == null && dto.PharmacistPrescriptionId == null)
                                {
                                    var newEntity = ObjectMapper.Map<PrescriptionItem>(dto);
                                    newEntity.PharmacistPrescriptionId = pharmacistPrescriptionId;
                                    newEntity.IsPrescribe = false;
                                    newEntity.PrescriptionId = null;

                                    await AdjustStockAsync(newEntity.MedicineId,dto.BatchId, newEntity.Qty);

                                    await _prescriptionItemRepository.InsertAsync(newEntity);
                                }
                            }
                        }
                    }
                    else
                    {
                        
                        pharmacistPrescriptionId = await Repository.InsertAndGetIdAsync(prescription);

                        foreach (var dto in withItemDto.pharmacistPrescriptionsListOfItem)
                        {
                            if (dto.PrescriptionId == null && dto.PharmacistPrescriptionId == null)
                            {
                                var newEntity = ObjectMapper.Map<PrescriptionItem>(dto);
                                newEntity.PharmacistPrescriptionId = pharmacistPrescriptionId;
                                newEntity.IsPrescribe = false;
                                newEntity.PrescriptionId = null;

                                await AdjustStockAsync(newEntity.MedicineId,dto.BatchId, newEntity.Qty);

                                await _prescriptionItemRepository.InsertAsync(newEntity);
                            }
                        }
                    }
                }
                else
                {
                    
                    await Repository.UpdateAsync(prescription);
                    pharmacistPrescriptionId = withItemDto.pharmacistPrescriptionsDto.Id;

                    var allItems = await _prescriptionItemRepository.GetAll()
                        .Where(x => x.PrescriptionId == prescription.PrescriptionId || x.PharmacistPrescriptionId == pharmacistPrescriptionId)
                        .ToListAsync();

                    foreach (var dto in withItemDto.pharmacistPrescriptionsListOfItem)
                    {
                        var existing = allItems.FirstOrDefault(x =>
                            x.MedicineId == dto.MedicineId && (
                                (x.PrescriptionId != null && x.PrescriptionId == dto.PrescriptionId) ||
                                (x.PrescriptionId == null && x.PharmacistPrescriptionId != null && x.PharmacistPrescriptionId == pharmacistPrescriptionId)
                            ));

                        if (existing != null)
                        {
                            
                            int oldQty = existing.Qty;
                            int newQty = dto.Qty;
                            int diff = newQty - oldQty;

                            if (diff != 0)
                                await AdjustStockAsync(existing.MedicineId,dto.BatchId, diff);

                            existing.Qty = newQty;
                            existing.Dosage = dto.Dosage;
                            existing.Frequency = dto.Frequency;
                            existing.Duration = dto.Duration;
                            existing.Instructions = dto.Instructions;
                            existing.PharmacistPrescriptionId = pharmacistPrescriptionId;

                            await _prescriptionItemRepository.UpdateAsync(existing);
                        }
                        else
                        {
                            if (dto.PrescriptionId == null && dto.PharmacistPrescriptionId == null)
                            {
                                var newEntity = ObjectMapper.Map<PrescriptionItem>(dto);
                                newEntity.PharmacistPrescriptionId = pharmacistPrescriptionId;
                                newEntity.IsPrescribe = false;
                                newEntity.PrescriptionId = null;

                                await AdjustStockAsync(newEntity.MedicineId,dto.BatchId, newEntity.Qty);

                                await _prescriptionItemRepository.InsertAsync(newEntity);
                            }
                        }
                    }
                }

                await CurrentUnitOfWork.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        private async Task AdjustStockAsync(long medicineMasterId, long batchId, int diff)
        {
            var stock = await _medicineStockRepository.FirstOrDefaultAsync(s =>
                s.Id == batchId &&
                s.MedicineMasterId == medicineMasterId &&
                !s.IsExpire &&
                (s.ExpiryDate == null || s.ExpiryDate >= DateTime.Today));

            if (stock == null)
                throw new UserFriendlyException($"No valid stock found for medicine {medicineMasterId} (Batch {batchId})");

            if (diff > 0)
            {
                if (stock.Quantity >= diff)
                {
                    stock.Quantity -= diff;
                }
                else
                {
                    throw new UserFriendlyException($"Not enough stock in batch {batchId} for medicine {medicineMasterId}");
                }
            }
            else if (diff < 0) 
            {
                stock.Quantity += Math.Abs(diff);
            }

            await _medicineStockRepository.UpdateAsync(stock);
        }


        [HttpGet]
        public async Task<EditPharmacistPrescriptionsWithItemsDto> GetPharmacistPrescriptionsById(long _id)
        {
            var details = await Repository.GetAllIncluding(
                                x => x.Prescriptions,
                                x => x.Prescriptions.Patient
                           )
                           .FirstOrDefaultAsync(x => x.Id == _id);

            if (details == null)
            {
                throw new UserFriendlyException($"No PharmacistPrescription found for Id {_id}");
            }

            var prescription = details.Prescriptions;

            var prescriptionItems = await _prescriptionItemRepository.GetAll()
                    .Where(i =>
                        i.PharmacistPrescriptionId == _id ||
                        (i.PrescriptionId == prescription.Id && i.PharmacistPrescriptionId == null)
                    ).ToListAsync();

            if (prescriptionItems == null || prescriptionItems.Count == 0)
            {
                prescriptionItems = await _prescriptionItemRepository.GetAll()
                    .Where(i => i.PrescriptionId == prescription.Id && i.PharmacistPrescriptionId == null)
                    .ToListAsync();
            }
            var mappedDetails = new EditPharmacistPrescriptionsWithItemsDto
            {
                PrescriptionId = prescription.Id,
                PatientId = prescription.PatientId,
                PharmacyNotes = details.PharmacyNotes,
                IsPaid = details.IsPaid,
                ReceiptNumber = details.ReceiptNumber,
                IssueDate = details.IssueDate,
                CollectionStatus = details.CollectionStatus,
                PatientName = prescription?.Patient?.FullName,
                PrescriptionItem = ObjectMapper.Map<List<PharmacistPrescriptionItemWithUnitPriceDto>>(prescriptionItems)
            };

            return mappedDetails;
        }

        [HttpGet]
        public async Task<ViewPharmacistPrescriptionsDto> ViewPharmacistPrescriptionsReceipt(long prescriptionId, long? pharmacistPrescriptionId)
        {
            var pharmacistPrescriptionsEntity = await Repository.GetAllIncluding(
                x => x.Prescriptions,
                x => x.Prescriptions.Patient,
                x => x.Prescriptions.Doctor,
                x => x.Nurse,
                x => x.Patient,
                x => x.PrescriptionItems
                ).FirstOrDefaultAsync(x => x.Id == pharmacistPrescriptionId);

            if (pharmacistPrescriptionsEntity == null)
                return null;

            var tenant = await _tenantManager.GetByIdAsync(AbpSession.TenantId.Value);

            // Use AutoMapper for mapping
            var dto = ObjectMapper.Map<ViewPharmacistPrescriptionsDto>(pharmacistPrescriptionsEntity);

            // Set tenant name manually
            dto.TenantName = tenant?.Name;

            return dto;
        }

        [HttpPost]
        public async Task MarkAsPickedUp(long? pharmacistPrescriptionId, long? pickedUpById, bool isPickedUpByNurse = false)
        {
            var prescription = await Repository.GetAllIncluding(
                                    x => x.Prescriptions.Patient.Admissions
                                )
                                .FirstOrDefaultAsync(x => x.Id == pharmacistPrescriptionId);

            if (prescription == null)
                return;

            prescription.CollectionStatus = CollectionStatus.PickedUp;

            if (isPickedUpByNurse)
            {
                prescription.PickedUpByNurse = pickedUpById;
            }
            else
            {
                prescription.PickedUpByPatient = prescription?.Prescriptions.Patient?.Id;
            }

            // ✅ Get medicine items only for this PharmacistPrescriptionId
            var medicineItems = await _prescriptionItemRepository
                .GetAll()
                .Where(x => x.PharmacistPrescriptionId == pharmacistPrescriptionId)
                .ToListAsync();

            if (medicineItems != null && medicineItems.Any())
            {
                foreach (var item in medicineItems)
                {

                    if (prescription.Prescriptions.IsEmergencyPrescription)
                    {
                        // 🔹 Emergency Charge Entry
                        var emergencyCharge = new EmergencyChargeEntry
                        {
                            TenantId = prescription.TenantId,
                            PatientId = prescription.Prescriptions.PatientId,
                            ChargeType = ChargeType.Medicine,
                            Description = $"Medicine: {item.MedicineName} ({item.Dosage})",
                            Amount = item.UnitPrice,
                            Quantity = item.Qty,
                            EntryDate = DateTime.Now,
                            IsProcessed = false,
                            PrescriptionId = prescription.PrescriptionId,
                            EmergencyCaseId = prescription.Prescriptions.EmergencyCaseId,
                            ReferenceId = prescription.Id
                        };

                        await _emergencyChargeRepository.InsertAsync(emergencyCharge);
                    }
                    else
                    {
                        // 🔹 IPD Charge Entry
                        var currentAdmission = prescription.Prescriptions.Patient?.Admissions
                            ?.FirstOrDefault(a => !a.IsDischarged);

                        var ipdCharge = new EMRSystem.IpdChargeEntry.IpdChargeEntry
                        {
                            TenantId = prescription.TenantId,
                            PatientId = prescription.Prescriptions.PatientId ?? 0,
                            ChargeType = ChargeType.Medicine,
                            Description = $"Medicine: {item.MedicineName} ({item.Dosage})",
                            Quantity = item.Qty,
                            Amount = item.UnitPrice,
                            EntryDate = DateTime.Now,
                            IsProcessed = false,
                            PrescriptionId = prescription.PrescriptionId,
                            AdmissionId = currentAdmission?.Id ?? 0,
                            ReferenceId = prescription.Id
                        };

                        await _ipdChargeRepository.InsertAsync(ipdCharge);
                    }
                }
            }

            await Repository.UpdateAsync(prescription);
        }

        [HttpGet]
        public async Task<List<PharmacistPrescriptionsDto>> GetPharmacistPrescriptionsByPatient(long patientID)
        {
            var list = await Repository.GetAllIncluding(x => x.Prescriptions.Doctor).Where(x => x.Prescriptions.PatientId == patientID).ToListAsync();
            var mappedList = ObjectMapper.Map<List<PharmacistPrescriptionsDto>>(list);
            return mappedList;
        }
    }
}
