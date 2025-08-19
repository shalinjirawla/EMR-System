using Abp.Application.Services;
using Abp.Domain.Repositories;
using Abp.Extensions;
using Abp.Linq.Extensions;
using Abp.UI;
using AutoMapper.Internal.Mappers;
using EMRSystem.Admissions.Dto;
using EMRSystem.Patients;
using EMRSystem.Room;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMRSystem.Admissions
{
    public class AdmissionAppService : AsyncCrudAppService<EMRSystem.Admission.Admission, AdmissionDto, long, PagedAdmissionResultRequestDto, CreateUpdateAdmissionDto, CreateUpdateAdmissionDto>, IAdmissionAppService
    {
        private readonly IRepository<EMRSystem.Admission.Admission, long> _repository;
        private readonly IRepository<EMRSystem.Doctors.Doctor, long> _doctorRepo;
        private readonly IRepository<EMRSystem.Nurses.Nurse, long> _nurseRepo;
        private readonly IRepository<Patient, long> _patientRepo;
        private readonly IRepository<EMRSystem.Room.Room, long> _roomRepo;

        public AdmissionAppService(
            IRepository<EMRSystem.Admission.Admission, long> repository,
            IRepository<EMRSystem.Doctors.Doctor, long> doctorRepo,
            IRepository<EMRSystem.Nurses.Nurse, long> nurseRepo,
             IRepository<EMRSystem.Room.Room, long> roomRepo,
            IRepository<Patient, long> patientRepo) : base(repository)
        {
            _repository = repository;
            _doctorRepo = doctorRepo;
            _nurseRepo = nurseRepo;
            _patientRepo = patientRepo;
            _roomRepo = roomRepo;
        }

        protected override IQueryable<EMRSystem.Admission.Admission> CreateFilteredQuery(PagedAdmissionResultRequestDto input)
        {
            return _repository.GetAll()
                .Include(x => x.Doctor)
                .Include(x => x.Nurse)
                .Include(x => x.Patient)
                .Include(x => x.Room)
                    .ThenInclude(r => r.RoomTypeMaster) 
                .WhereIf(!input.Keyword.IsNullOrWhiteSpace(),
                    x => x.Patient.FullName.Contains(input.Keyword));
        }


        public override async Task<AdmissionDto> CreateAsync(CreateUpdateAdmissionDto input)
        {
            // Validate patient
            var patient = await _patientRepo.FirstOrDefaultAsync(input.PatientId);
            if (patient == null)
            {
                throw new UserFriendlyException("Invalid patient");
            }
            else
            {
                patient.IsAdmitted = true;
                await _patientRepo.UpdateAsync(patient);
            }

            // Validate and update Room status
            if (input.RoomId.HasValue)
            {
                var room = await _roomRepo.FirstOrDefaultAsync(input.RoomId.Value);
                if (room == null)
                    throw new UserFriendlyException("Invalid room");

                room.Status = RoomStatus.Occupied;
                await _roomRepo.UpdateAsync(room);
            }
            

            // Map and insert admission
            var admission = ObjectMapper.Map<EMRSystem.Admission.Admission>(input);
            await _repository.InsertAsync(admission);

            await CurrentUnitOfWork.SaveChangesAsync();

            return MapToEntityDto(admission);
        }

    }
}
