using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Domain.Repositories;
using Abp.Extensions;
using Abp.Linq.Extensions;
using Abp.UI;
using AutoMapper.Internal.Mappers;
using EMRSystem.Admissions.Dto;
using EMRSystem.Deposit;
using EMRSystem.Insurances;
using EMRSystem.IpdChargeEntry;
using EMRSystem.PatientDischarge;
using EMRSystem.Patients;
using EMRSystem.Room;
using EMRSystem.RoomMaster;
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
        private readonly IRepository<EMRSystem.PatientDischarge.PatientDischarge, long> _patientDischargeRepository;
        private readonly IRepository<Patient, long> _patientRepo;
        private readonly IRepository<EMRSystem.Room.Room, long> _roomRepo;
        private readonly IRepository<Bed, long> _bedRepo;
        private readonly IRepository<PatientDeposit, long> _patientDepositRepo;
        private readonly IRepository<EMRSystem.IpdChargeEntry.IpdChargeEntry, long> _ipdChargeRepo;
        private readonly IRepository<RoomTypeMaster, long> _roomTypeRepo;
        private readonly IRepository<PatientInsurance, long> _patientInsuranceRepository;

        public AdmissionAppService(
            IRepository<EMRSystem.Admission.Admission, long> repository,
            IRepository<EMRSystem.PatientDischarge.PatientDischarge, long> patientDischargeRepository,
            IRepository<EMRSystem.Room.Room, long> roomRepo,
            IRepository<RoomTypeMaster, long> roomTypeRepo,
            IRepository<Bed, long> bedRepo,
            IRepository<PatientDeposit, long> patientDepositRepo,
            IRepository<PatientInsurance, long> patientInsuranceRepository,
            IRepository<EMRSystem.IpdChargeEntry.IpdChargeEntry, long> ipdChargeRepo,
            IRepository<Patient, long> patientRepo) : base(repository)
        {
            _repository = repository;
            _patientRepo = patientRepo;
            _patientDepositRepo = patientDepositRepo;
            _ipdChargeRepo = ipdChargeRepo;
            _roomRepo = roomRepo;
            _patientInsuranceRepository = patientInsuranceRepository;
            _roomTypeRepo = roomTypeRepo;
            _bedRepo = bedRepo;
            _patientDischargeRepository = patientDischargeRepository;
        }

        protected override IQueryable<EMRSystem.Admission.Admission> CreateFilteredQuery(PagedAdmissionResultRequestDto input)
        {
            return _repository.GetAll()
                .Include(x => x.Doctor)
                .Include(x => x.Nurse)
                .Include(x => x.Patient)
                .Include(x => x.Bed)
                .Include(x => x.Room)
                    .ThenInclude(r => r.RoomTypeMaster)
                .Where(x => x.IsDischarged == false)
                .WhereIf(!input.Keyword.IsNullOrWhiteSpace(),
                    x => x.Patient.FullName.Contains(input.Keyword));
        }

        public override async Task<AdmissionDto> GetAsync(EntityDto<long> input)
        {
            var query = await _repository.GetAll()
                .Include(x => x.Doctor)
                .Include(x => x.Nurse)
                .Include(x => x.Patient)
                .Include(x => x.Room)
                    .ThenInclude(r => r.RoomTypeMaster)
                .Include(x => x.Bed)
                .Include(x => x.PatientInsurance)
                    .ThenInclude(pi => pi.InsuranceMaster)
                .Where(x => x.Id == input.Id)
                .FirstOrDefaultAsync();

            if (query == null)
                throw new UserFriendlyException("Admission not found");

            var dto = MapToEntityDto(query);

            return dto;
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
            if (input.BedId.HasValue)
            {
                var bed = await _bedRepo.FirstOrDefaultAsync(input.BedId.Value);
                if (bed == null)
                    throw new UserFriendlyException("Invalid bed");

                bed.Status = BedStatus.Occupied;
                await _bedRepo.UpdateAsync(bed);
            }

            // ✅ Step 1: Map and insert admission
            var admission = ObjectMapper.Map<EMRSystem.Admission.Admission>(input);
            admission.PatientInsuranceId = null;

            var newAdmissionId = await _repository.InsertAndGetIdAsync(admission);
            await CurrentUnitOfWork.SaveChangesAsync();

            // ✅ Step 2: Create PatientDeposit entry
            var deposit = new PatientDeposit
            {
                TenantId = admission.TenantId,
                PatientId = admission.PatientId,
                TotalCreditAmount = 0,
                TotalDebitAmount = 0,
                TotalBalance = 0
            };
            await _patientDepositRepo.InsertAsync(deposit);

            var room = await _roomRepo.GetAsync(admission.RoomId);
            var roomType = await _roomTypeRepo.GetAsync(room.RoomTypeMasterId);
            var ipdCharge = new EMRSystem.IpdChargeEntry.IpdChargeEntry
            {
                TenantId = admission.TenantId,
                AdmissionId = admission.Id,
                PatientId = admission.PatientId,
                ChargeType = ChargeType.Room,
                Description = $"Room Charge (Room No: {room.RoomNumber})",
                Quantity = 1,
                Amount = roomType.DefaultPricePerDay, // daily charge
                EntryDate = DateTime.Now,
                IsProcessed = false,
                ReferenceId = room.Id
            };
            await _ipdChargeRepo.InsertAsync(ipdCharge);

            await CurrentUnitOfWork.SaveChangesAsync();

            var res = MapToEntityDto(admission);

            var patientDischarge = new EMRSystem.PatientDischarge.PatientDischarge();
            patientDischarge.TenantId = AbpSession.TenantId.Value;
            patientDischarge.AdmissionId = admission.Id;
            patientDischarge.PatientId = admission.PatientId;
            patientDischarge.DischargeStatus = DischargeStatus.Pending;
            patientDischarge.DischargeDate = null;
            patientDischarge.DischargeSummary = null;
            patientDischarge.DoctorId = null;
            await _patientDischargeRepository.InsertAsync(patientDischarge);

            if (input.BillingMode == BillingMethod.InsuranceOnly || input.BillingMode == BillingMethod.InsuranceSelfPay)
            {
                if (input.PatientInsurance != null)
                {
                    var insurance = ObjectMapper.Map<PatientInsurance>(input.PatientInsurance);
                    insurance.PatientId = admission.PatientId;
                    insurance.TenantId = admission.TenantId;

                    var insuranceId = await _patientInsuranceRepository.InsertAndGetIdAsync(insurance);
                    await CurrentUnitOfWork.SaveChangesAsync();

                    // ✅ Update admission with insurance ID
                    admission.PatientInsuranceId = insuranceId;
                    await _repository.UpdateAsync(admission);
                    await CurrentUnitOfWork.SaveChangesAsync();
                }
            }


            return res;
        }

        public override async Task<AdmissionDto> UpdateAsync(CreateUpdateAdmissionDto input)
        {
            var admission = await _repository.GetAsync(input.Id);
            if (admission == null)
                throw new UserFriendlyException("Admission not found");

            // ✅ Handle bed change safely BEFORE mapping
            if (input.BedId.HasValue)
            {
                if (admission.BedId.HasValue && admission.BedId.Value != input.BedId.Value)
                {
                    var oldBed = await _bedRepo.FirstOrDefaultAsync(admission.BedId.Value);
                    if (oldBed != null)
                    {
                        oldBed.Status = BedStatus.Available;
                        await _bedRepo.UpdateAsync(oldBed);
                    }
                }

                var newBed = await _bedRepo.FirstOrDefaultAsync(input.BedId.Value);
                if (newBed == null)
                    throw new UserFriendlyException("Invalid bed");

                newBed.Status = BedStatus.Occupied;
                await _bedRepo.UpdateAsync(newBed);
                admission.BedId = input.BedId.Value;
            }

            // ✅ Keep old bedId safe before mapper
            var oldBedId = admission.BedId;

            // ✅ Map remaining admission properties (except bedId)
            ObjectMapper.Map(input, admission);
            admission.BedId = oldBedId;

            // ---------------- INSURANCE LOGIC ----------------

            // If NO insurance (Self-pay)
            if (input.BillingMode == BillingMethod.SelfPay)
            {
                var existingInsurance = await _patientInsuranceRepository.FirstOrDefaultAsync(x =>
                    x.PatientId == admission.PatientId &&
                    x.IsActive &&
                    x.TenantId == admission.TenantId);

                if (existingInsurance != null)
                {
                    existingInsurance.IsActive = false;
                    await _patientInsuranceRepository.UpdateAsync(existingInsurance);
                }
            }
            else
            {
                // Insurance billing modes (Insurance + Self / Cashless)

                if (input.PatientInsurance != null)
                {
                    // Editing existing insurance
                    if (input.PatientInsurance.Id > 0)
                    {
                        var insurance = await _patientInsuranceRepository.GetAsync(input.PatientInsurance.Id);

                        insurance.InsuranceId = input.PatientInsurance.InsuranceId;
                        insurance.PolicyNumber = input.PatientInsurance.PolicyNumber;
                        insurance.CoverageLimit = input.PatientInsurance.CoverageLimit;
                        insurance.CoPayPercentage = input.PatientInsurance.CoPayPercentage;
                        insurance.IsActive = true;

                        await _patientInsuranceRepository.UpdateAsync(insurance);
                    }
                    else
                    {
                        // New insurance during update
                        var newInsurance = new PatientInsurance
                        {
                            TenantId = admission.TenantId,
                            PatientId = admission.PatientId, // ✅ keep this
                            InsuranceId = input.PatientInsurance.InsuranceId,
                            PolicyNumber = input.PatientInsurance.PolicyNumber,
                            CoverageLimit = input.PatientInsurance.CoverageLimit,
                            CoPayPercentage = input.PatientInsurance.CoPayPercentage,
                            IsActive = true
                        };

                        await _patientInsuranceRepository.InsertAsync(newInsurance);
                    }
                }
            }

            // ✅ Save changes
            await _repository.UpdateAsync(admission);
            await CurrentUnitOfWork.SaveChangesAsync();

            return MapToEntityDto(admission);
        }

    }
}
