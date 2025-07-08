using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Domain.Entities;
using Abp.Domain.Repositories;
using Abp.Extensions;
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
        protected override IQueryable<MedicineOrder> CreateFilteredQuery(PagedAndSortedResultRequestDto input)
        {
            try
            {

                //var userId = AbpSession.UserId;
                //var doctor = _doctorAppService.GetDoctorDetailsByAbpUserID(userId.Value);

                return Repository
                    .GetAll()
                    .Include(x => x.Patient)
                    .Include(x => x.Nurse)
                    .Select(x => new MedicineOrder
                    {
                        Id = x.Id,
                        TenantId = x.TenantId,
                        Status= x.Status,
                        Priority=x.Priority,
                        Patient = x.Patient == null ? null : new Patient
                        {
                            Id = x.Patient.Id,
                            FullName = x.Patient.FullName
                        },
                        Nurse = x.Nurse == null ? null : new EMRSystem.Nurses.Nurse
                        {
                            Id = x.Nurse.Id,
                            FullName = x.Nurse.FullName
                        }
                    });
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
