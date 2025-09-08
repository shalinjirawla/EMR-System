using Abp.Application.Services.Dto;
using Abp.Application.Services;
using Abp.Domain.Repositories;
using EMRSystem.Pharmacist.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Abp.UI;
using EMRSystem.Appointments;
using EMRSystem.EmergencyProcedure;
using EMRSystem.IpdChargeEntry;
using EMRSystem.LabReports;
using EMRSystem.MedicineOrder;
using EMRSystem.Prescriptions.Dto;
using EMRSystem.Prescriptions;
using EMRSystem.LabReport.Dto;
using EMRSystem.MultiTenancy;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using EMRSystem.Patients;
using EMRSystem.Pharmacists;
using Microsoft.AspNetCore.Http.HttpResults;
using Abp.EntityFrameworkCore.Repositories;
using EMRSystem.Admission;
using static Castle.MicroKernel.ModelBuilder.Descriptors.InterceptorDescriptor;

namespace EMRSystem.Pharmacist
{

    public class PharmacistPrescriptionsAppService : AsyncCrudAppService<EMRSystem.Pharmacists.PharmacistPrescriptions, PharmacistPrescriptionsDto, long, PagedAndSortedResultRequestDto, CreateUpdatePharmacistPrescriptionsDto, CreateUpdatePharmacistPrescriptionsDto>,
     IPharmacistPrescriptionsAppService
    {

        private readonly IRepository<EMRSystem.Pharmacists.PharmacistInventory, long> _pharmacistInventoryRepository;
        private readonly IRepository<EMRSystem.Prescriptions.PrescriptionItem, long> _prescriptionItemRepository;
        private readonly TenantManager _tenantManager;
        public PharmacistPrescriptionsAppService(IRepository<EMRSystem.Pharmacists.PharmacistPrescriptions, long> repository,
            IRepository<Pharmacists.PharmacistInventory, long> pharmacistInventoryRepository, TenantManager tenantManager,
            IRepository<PrescriptionItem, long> prescriptionItemRepository) : base(repository)
        {
            _pharmacistInventoryRepository = pharmacistInventoryRepository;
            _tenantManager = tenantManager;
            _prescriptionItemRepository = prescriptionItemRepository;
        }

        [HttpGet]
        public async Task<PagedResultDto<PharmacistPrescriptionsDto>> GetPrescriptionFulfillment(PagedAndSortedResultRequestDto input)
        {
            try
            {
                var list = await Repository.GetAllIncluding
                        (
                        x => x.Prescriptions,
                        x => x.Prescriptions.Patient,
                        x => x.Prescriptions.Doctor,
                        x => x.Prescriptions.Items
                        )
                        .Where(x => x.Prescriptions != null && x.Prescriptions.Items != null)
                        .ToListAsync();
                var mappedItems = ObjectMapper.Map<List<PharmacistPrescriptionsDto>>(list);
                mappedItems.ForEach(x =>
                {
                    if (x?.prescriptionItems != null)
                    {
                        foreach (var item in x.prescriptionItems)
                        {
                            item.UnitPrice = _pharmacistInventoryRepository.Get(item.MedicineId).SellingPrice;
                        }
                    }
                });
                return new PagedResultDto<PharmacistPrescriptionsDto>(
                    list.Count,
                    mappedItems
                );
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException("Error in GetPrescriptionFulfillment", ex);
            }
        }
        [HttpPost]
        public async Task CreatePharmacistPrescriptionsWithItem(CreatePharmacistPrescriptionsWithItemDto withItemDto)
        {
            try
            {
                var prescription = ObjectMapper.Map<EMRSystem.Pharmacists.PharmacistPrescriptions>(withItemDto.pharmacistPrescriptionsDto);
                prescription.IsPaid = true;
                long? pharmacistPrescriptionId = null;
                if (withItemDto.pharmacistPrescriptionsDto.Id <= 0)
                {
                    var isAlreadyExist = await Repository.GetAll().FirstOrDefaultAsync(x => x.PrescriptionId == prescription.PrescriptionId);
                    if (isAlreadyExist != null && !isAlreadyExist.IsPaid && isAlreadyExist.CollectionStatus == CollectionStatus.NotPickedUp)
                    {
                        await Repository.DeleteAsync(isAlreadyExist);
                    }
                    pharmacistPrescriptionId = await Repository.InsertAndGetIdAsync(prescription);
                    var entity = ObjectMapper.Map<List<PrescriptionItem>>(withItemDto.pharmacistPrescriptionsListOfItem);
                    if (entity.Count > 0)
                    {
                        foreach (var x in entity)
                        {
                            x.PharmacistPrescriptionId = pharmacistPrescriptionId;
                            x.IsPrescribe = false;
                            var inventoryItem = await _pharmacistInventoryRepository.FirstOrDefaultAsync(t => t.Id == x.Id);
                            if (inventoryItem != null)
                            {
                                if (inventoryItem.Stock >= x.Qty)
                                {
                                    inventoryItem.Stock -= x.Qty;
                                    await _pharmacistInventoryRepository.UpdateAsync(inventoryItem);
                                }
                            }
                        }
                        await _prescriptionItemRepository.InsertRangeAsync(entity);
                    }
                }
                else
                {
                    await Repository.UpdateAsync(prescription);
                    pharmacistPrescriptionId = withItemDto.pharmacistPrescriptionsDto.Id;


                    var getExistingListOfItems = await _prescriptionItemRepository.GetAll()
                                                .Where(x => x.PharmacistPrescriptionId == pharmacistPrescriptionId)
                                                .ToListAsync();


                    // Step 2: Restore stock for existing items
                    foreach (var oldItem in getExistingListOfItems)
                    {
                        var inventoryItem = await _pharmacistInventoryRepository.FirstOrDefaultAsync(x => x.Id == oldItem.MedicineId);
                        if (inventoryItem != null)
                        {
                            inventoryItem.Stock += oldItem.Qty; // restore stock
                            await _pharmacistInventoryRepository.UpdateAsync(inventoryItem);
                        }
                    }


                    if (getExistingListOfItems.Count > 0)
                        _prescriptionItemRepository.RemoveRange(getExistingListOfItems);

                    var entity = ObjectMapper.Map<List<PrescriptionItem>>(withItemDto.pharmacistPrescriptionsListOfItem);
                    if (entity.Count > 0)
                    {
                        foreach (var x in entity)
                        {
                            x.PharmacistPrescriptionId = pharmacistPrescriptionId;
                            x.IsPrescribe = false;
                            var inventoryItem = await _pharmacistInventoryRepository.FirstOrDefaultAsync(x => x.Id == x.Id);
                            if (inventoryItem != null)
                            {
                                if (inventoryItem.Stock >= x.Qty)
                                {
                                    inventoryItem.Stock -= x.Qty;
                                    await _pharmacistInventoryRepository.UpdateAsync(inventoryItem);
                                }
                            }
                        }
                        await _prescriptionItemRepository.InsertRangeAsync(entity);
                    }
                }

                await CurrentUnitOfWork.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpGet]
        public async Task<EditPharmacistPrescriptionsWithItemsDto> GetPharmacistPrescriptionsById(long _id)
        {
            var details = await Repository.GetAllIncluding(
                                            x => x.Prescriptions,
                                            x => x.Prescriptions.Patient,
                                            x => x.Prescriptions.Items.Where(x => x.PharmacistPrescriptionId == null)
                                            ).FirstOrDefaultAsync(x => x.Id == _id);
            var prescription = details.Prescriptions;
            var mappedDetails = new EditPharmacistPrescriptionsWithItemsDto();
            mappedDetails.PrescriptionId = prescription.Id;
            mappedDetails.PatientId = prescription.PatientId;
            mappedDetails.PharmacyNotes = details.PharmacyNotes;
            mappedDetails.IssueDate = prescription.IssueDate;
            mappedDetails.CollectionStatus = details.CollectionStatus;
            mappedDetails.PatientName = prescription?.Patient?.FullName;
            mappedDetails.PrescriptionItem = ObjectMapper.Map<List<PharmacistPrescriptionItemWithUnitPriceDto>>(prescription.Items);
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
            var dto = new ViewPharmacistPrescriptionsDto();
            dto.TenantName = tenant?.Name;
            dto.PharmacistPrescriptionId = pharmacistPrescriptionsEntity.Id;
            dto.PrescriptionId = pharmacistPrescriptionsEntity.PrescriptionId.Value;
            dto.PatientId = pharmacistPrescriptionsEntity.Prescriptions?.Patient?.Id ?? 0;
            dto.PatientName = pharmacistPrescriptionsEntity.Prescriptions?.Patient?.FullName;
            dto.PatientDateOfBirth = pharmacistPrescriptionsEntity.Prescriptions?.Patient?.DateOfBirth;
            dto.Gender = pharmacistPrescriptionsEntity.Prescriptions?.Patient?.Gender;
            dto.DoctorName = pharmacistPrescriptionsEntity?.Prescriptions?.Doctor?.FullName;
            dto.DoctorRegistrationNumber = pharmacistPrescriptionsEntity.Prescriptions.Doctor?.RegistrationNumber;
            dto.PharmacyNotes = pharmacistPrescriptionsEntity.PharmacyNotes;
            dto.IsPaid = pharmacistPrescriptionsEntity.IsPaid;
            dto.CollectionStatus = pharmacistPrescriptionsEntity.CollectionStatus;
            dto.IssueDate = pharmacistPrescriptionsEntity.IssueDate.Value;
            dto.PickedUpByNurseId = pharmacistPrescriptionsEntity.PickedUpByNurse;
            dto.PickedUpByNurse = pharmacistPrescriptionsEntity.Nurse?.FullName;
            dto.PickedUpByPatientId = pharmacistPrescriptionsEntity.PickedUpByPatient;
            dto.PickedUpByPatient = pharmacistPrescriptionsEntity.Patient?.FullName;
            dto.GrandTotal = pharmacistPrescriptionsEntity.GrandTotal;
            dto.PrescriptionItems = pharmacistPrescriptionsEntity.PrescriptionItems?.Select(i => new PrescriptionItemDto
            {
                MedicineName = i.MedicineName,
                Dosage = i.Dosage,
                Frequency = i.Frequency,
                Duration = i.Duration,
                Instructions = i.Instructions,
                IsPrescribe = i.IsPrescribe,
                Qty = i.Qty,
                UnitPrice = i.UnitPrice,
                PharmacistPrescriptionId = i.PharmacistPrescriptionId
            }).ToList();

            return dto;
        }

        [HttpPost]
        public async Task MarkAsPickedUp(long? pharmacistPrescriptionId, long? pickedUpById, bool isPickedUpByNurse = false)
        {
            var prescription = await Repository.GetAllIncluding(x => x.Prescriptions.Patient).FirstOrDefaultAsync(x => x.Id == pharmacistPrescriptionId);
            if (prescription != null)
            {
                prescription.CollectionStatus = CollectionStatus.PickedUp;
                if (isPickedUpByNurse)
                {
                    prescription.PickedUpByNurse = pickedUpById;
                }
                else
                {
                    prescription.PickedUpByPatient = prescription?.Prescriptions.Patient?.Id;
                }
                await Repository.UpdateAsync(prescription);
            }
        }
    }
}
