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
                        entity.ForEach(x =>
                        {
                            x.PharmacistPrescriptionId = pharmacistPrescriptionId;
                            x.IsPrescribe = false;
                        });
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
                    if (getExistingListOfItems.Count > 0)
                        _prescriptionItemRepository.RemoveRange(getExistingListOfItems);

                    var entity = ObjectMapper.Map<List<PrescriptionItem>>(withItemDto.pharmacistPrescriptionsListOfItem);
                    if (entity.Count > 0)
                    {
                        entity.ForEach(x =>
                        {
                            x.PharmacistPrescriptionId = pharmacistPrescriptionId;
                            x.IsPrescribe = false;
                        });
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
            var query = await Repository.GetAllIncludingAsync(
                x => x.Prescriptions,
                x => x.Prescriptions.PharmacistPrescriptions,
                x => x.Prescriptions.Patient,
                x => x.Prescriptions.Doctor,
                x => x.Prescriptions,
                x => x.Prescriptions.Consultation_Requests,
                x => x.Prescriptions.SelectedEmergencyProcedureses,
                x => x.Prescriptions.Items.Where(x => x.PharmacistPrescriptionId == pharmacistPrescriptionId)
                );

            var entity = await query
                    .Where(x => x.PrescriptionId == prescriptionId)
                    .Select(x => x.Prescriptions)
                    .FirstOrDefaultAsync();

            if (entity == null)
                return null;
            var tenant = await _tenantManager.GetByIdAsync(AbpSession.TenantId.Value);
            var dto = new ViewPharmacistPrescriptionsDto
            {
                TenantName = tenant?.Name,
                PharmacistPrescriptionId = entity.PharmacistPrescriptions.ToList()[0].Id,
                PrescriptionId = entity.Id,
                PatientId = entity.Patient?.Id ?? 0,
                PatientName = entity.Patient?.FullName,
                PatientDateOfBirth = entity.Patient?.DateOfBirth,
                Gender = entity.Patient?.Gender,
                DoctorName = entity.Doctor?.FullName,
                DoctorRegistrationNumber = entity.Doctor?.RegistrationNumber,
                PharmacyNotes = entity.PharmacistPrescriptions.ToList()[0].PharmacyNotes,
                IsPaid = entity.PharmacistPrescriptions.ToList()[0].IsPaid,
                CollectionStatus = entity.PharmacistPrescriptions.ToList()[0].CollectionStatus,
                IssueDate = entity.PharmacistPrescriptions.ToList()[0].IssueDate.Value,
                PickedUpByNurseId = entity.PharmacistPrescriptions?.ToList()[0].Nurse?.Id,
                PickedUpByNurse = entity.PharmacistPrescriptions?.ToList()[0].Nurse?.FullName,
                PickedUpByPatientId = entity.PharmacistPrescriptions?.ToList()[0].Patient?.Id,
                PickedUpByPatient = entity.PharmacistPrescriptions?.ToList()[0].Patient?.FullName,
                GrandTotal = entity.PharmacistPrescriptions.ToList()[0].GrandTotal,
                PrescriptionItems = entity.Items?.Select(i => new PrescriptionItemDto
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
                }).ToList()
            };

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
