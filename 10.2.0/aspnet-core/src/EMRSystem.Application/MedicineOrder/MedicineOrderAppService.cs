using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Domain.Entities;
using Abp.Domain.Repositories;
using Abp.Extensions;
using Abp.UI;
using EMRSystem.Appointments;
using EMRSystem.MedicineOrder.Dto;
using EMRSystem.Patients;
using EMRSystem.Prescriptions;
using EMRSystem.Prescriptions.Dto;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMRSystem.MedicineOrder
{
    public class MedicineOrderAppService : AsyncCrudAppService<MedicineOrder, MedicineOrderDto, long, PagedAndSortedResultRequestDto, CreateUpdateMedicineOrderDto, CreateUpdateMedicineOrderDto>,
  IMedicineOrderAppService
    {
        public MedicineOrderAppService(IRepository<MedicineOrder, long> repository) : base(repository)
        {
        }
        public async Task CreateMedicineOrderWithItemAsync(CreateUpdateMedicineOrderDto input)
        {
            var medicineOrder = ObjectMapper.Map<MedicineOrder>(input);

            medicineOrder.Items = input.Items.Select(item =>
                ObjectMapper.Map<MedicineOrderItem>(item)).ToList();

            await Repository.InsertAsync(medicineOrder);
            await CurrentUnitOfWork.SaveChangesAsync();
        }
        public async Task UpdateMedicineOrderWithItemAsync(CreateUpdateMedicineOrderDto input)
        {
            // First get the existing prescription with its items
            var existingmedicineOrder = await Repository.GetAllIncluding(
                p => p.Items
            ).FirstOrDefaultAsync(p => p.Id == input.Id);

            if (existingmedicineOrder == null)
            {
                throw new UserFriendlyException("Prescription not found");
            }

            // Map the input to the existing prescription
            ObjectMapper.Map(input, existingmedicineOrder);

            // Handle Items update
            var existingItems = existingmedicineOrder.Items.ToList();

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
                    var newItem = ObjectMapper.Map<MedicineOrderItem>(inputItem);
                    existingmedicineOrder.Items.Add(newItem);
                }
            }

            // Remove items that are no longer in the input
            var itemsToRemove = existingItems.Where(ei => !input.Items.Any(ii => ii.Id == ei.Id)).ToList();
            foreach (var item in itemsToRemove)
            {
                existingmedicineOrder.Items.Remove(item);
            }

          

            await Repository.UpdateAsync(existingmedicineOrder);
            await CurrentUnitOfWork.SaveChangesAsync();
        }
        protected override IQueryable<MedicineOrder> CreateFilteredQuery(PagedAndSortedResultRequestDto input)
        {
            try
            {
                var entity = Repository
                    .GetAllIncluding(
                        x => x.Patient,
                        x => x.Nurse,
                        x => x.Items // Include the Items collection
                    )
                    .Include(x => x.Items) // This line ensures EF sees it clearly
                        .ThenInclude(i => i.Medicine); // Properly include nested Medicine

                if (entity == null)
                {
                    throw new EntityNotFoundException(nameof(MedicineOrder));
                }

                return entity;
            }
            catch (SqlException)
            {
                throw;
            }
            catch (Exception)
            {
                throw;
            }
        }


        public async Task<CreateUpdateMedicineOrderDto> GetOrderDetailsById(long id)
        {
            var entity = await Repository
                .GetAllIncluding(x => x.Patient, x => x.Nurse, x => x.Items)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (entity == null)
            {
                throw new EntityNotFoundException(typeof(MedicineOrder), id);
            }

            var dto = ObjectMapper.Map<CreateUpdateMedicineOrderDto>(entity);
            return dto;
        }


        //public async Task<CreateUpdateMedicineOrderDto> GetOrderDetailsById(long id)
        //{
        //    var query = await Repository.GetAllIncludingAsync(
        //        x => x.Patient,
        //        x => x.Nurse,
        //        x => x.Items); // Don't forget to include Items if needed

        //    var details = await query
        //        .Where(x => x.Id == id)
        //        .Select(x => new MedicineOrder
        //        {
        //            Id = x.Id,
        //            TenantId = x.TenantId,
        //            Status = x.Status,
        //            NurseId=x.NurseId,
        //            PatientId=x.PatientId,
        //            Priority = x.Priority,
        //            Patient = x.Patient == null ? null : new Patient
        //            {
        //                Id = x.Patient.Id,
        //                FullName = x.Patient.FullName
        //            },
        //            Nurse = x.Nurse == null ? null : new EMRSystem.Nurses.Nurse
        //            {
        //                Id = x.Nurse.Id,
        //                FullName = x.Nurse.FullName
        //            },
        //            Items = x.Items.Select(i => new MedicineOrderItem
        //            {
        //                Id = i.Id,
        //                MedicineOrderId=i.MedicineOrderId,
        //                MedicineId = i.MedicineId,
        //                Quantity=i.Quantity,
        //                Dosage = i.Dosage
        //            }).ToList(),

        //        })
        //        .FirstOrDefaultAsync();

        //    if (details == null)
        //    {
        //        throw new EntityNotFoundException(typeof(MedicineOrder), id);
        //    }

        //    var order = ObjectMapper.Map<CreateUpdateMedicineOrderDto>(details);


        //    return order;
        //}
    }
}
