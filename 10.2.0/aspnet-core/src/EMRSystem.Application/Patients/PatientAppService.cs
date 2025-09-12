using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Collections.Extensions;
using Abp.Domain.Repositories;
using Abp.Extensions;
using Abp.Linq.Extensions;
using Abp.UI;
using EMRSystem.Appointments;
using EMRSystem.Appointments.Dto;
using EMRSystem.Authorization.Users;
using EMRSystem.Doctor;
using EMRSystem.Doctors;
using EMRSystem.Invoices;
using EMRSystem.Nurse;
using EMRSystem.Nurse.Dto;
using EMRSystem.Nurses;
using EMRSystem.Patients.Dto;
using EMRSystem.Prescriptions;
using EMRSystem.Room;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Stripe.Checkout;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
namespace EMRSystem.Patients
{
    //[AbpAuthorize("Pages.Doctors.Patients")]

    public class PatientAppService : AsyncCrudAppService<Patient, PatientDto, long, PagedPatientResultRequestDto, CreateUpdatePatientDto, CreateUpdatePatientDto>,
    IPatientAppService
    {
        private readonly UserManager _userManager;
        private readonly IDoctorAppService _doctorAppService;
        private readonly INurseAppService _nurseAppService;
        private readonly IRepository<EMRSystem.Room.Room, long> _roomRepository;
        public PatientAppService(IRepository<Patient, long> repository, UserManager userManager, IDoctorAppService doctorAppService, INurseAppService nurseAppService, IRepository<Room.Room, long> roomRepository) : base(repository)
        {
            _userManager = userManager;
            _doctorAppService = doctorAppService;
            _nurseAppService = nurseAppService;
            _roomRepository = roomRepository;
        }

        protected override IQueryable<Patient> CreateFilteredQuery(PagedPatientResultRequestDto input)
        {
            return Repository.GetAll();
        }

        public override async Task<PatientDto> CreateAsync(CreateUpdatePatientDto input)
        {
            CheckCreatePermission();


            var patient = ObjectMapper.Map<Patient>(input);
                await Repository.InsertAsync(patient);
                await CurrentUnitOfWork.SaveChangesAsync();
                return MapToEntityDto(patient);
        }


        //public override async Task<PatientDto> UpdateAsync(CreateUpdatePatientDto input)
        //{
        //    CheckUpdatePermission();

        //    var patient = await Repository.GetAsync(input.Id);
        //    var oldRoomId = patient.RoomId;       

        //    //MapToEntity(input, patient);        

        //    using (CurrentUnitOfWork.SetTenantId(AbpSession.TenantId))
        //    {

        //        if (oldRoomId != input.RoomId)
        //        {

        //            if (oldRoomId.HasValue)
        //            {
        //                var oldRoom = await _roomRepository.GetAsync(oldRoomId.Value);
        //                oldRoom.Status = RoomStatus.Available;
        //            }


        //            if (patient.RoomId.HasValue)
        //            {
        //                var newRoom = await _roomRepository.GetAsync(patient.RoomId.Value);

        //                if (newRoom.Status != RoomStatus.Available)
        //                    throw new UserFriendlyException("Selected room is already occupied.");

        //                newRoom.Status = RoomStatus.Occupied;
        //            }
        //        }
        //        patient.RoomId = input.RoomId;

        //        await Repository.UpdateAsync(patient);
        //        await CurrentUnitOfWork.SaveChangesAsync();
        //    }

        //    return MapToEntityDto(patient);
        //}

        public override async Task<PatientDto> UpdateAsync(CreateUpdatePatientDto input)
        {
            CheckUpdatePermission();
            var user = await Repository.GetAsync(input.Id);
            MapToEntity(input, user);
            await Repository.UpdateAsync(user);
            return await GetAsync(input);
        }


        public override async Task DeleteAsync(EntityDto<long> input)
        {
            var user = await Repository.GetAsync(input.Id);
            await Repository.DeleteAsync(user);
        }

        [HttpGet]
        public async Task<PagedResultDto<PatientsForDoctorAndNurseDto>> PatientsForNurse(PagedPatientResultRequestDto input)
        {
            long nurseID = 0;
            bool isAdmin = false;
            if (AbpSession.UserId.HasValue)
            {
                var nurse = _nurseAppService.GetNurseDetailsByAbpUserID(AbpSession.UserId.Value);
                if (nurse != null)
                    nurseID = nurse.Id;
                else
                    isAdmin = true;
            }

            var query = Repository.GetAll()
                //.Include(x => x.Doctors)
                .Include(x => x.AbpUser)
                .Include(x=>x._PatientDischarge)
                .Include(p => p.Admissions)
                  .ThenInclude(a => a.Doctor)
                .Include(p => p.Admissions)
                  .ThenInclude(a => a.Nurse)
                .Where(p => p.IsAdmitted)
                .WhereIf(!isAdmin && nurseID > 0, p => p.Admissions.Any(a => a.NurseId == nurseID))
                //.WhereIf(nurseID > 0, i => i.AssignedNurseId == nurseID)
                .WhereIf(!input.Keyword.IsNullOrWhiteSpace(),
                    x => x.FullName.Contains(input.Keyword) ||
                         (x.AbpUser != null && x.AbpUser.EmailAddress.Contains(input.Keyword)));

            var totalCount = await query.CountAsync();

            // Apply sorting before pagination
            var orderedQuery = input.Sorting switch
            {
                "FullName" => query.OrderBy(x => x.FullName),
                "FullName desc" => query.OrderByDescending(x => x.FullName),
                "AbpUser.EmailAddress" => query.OrderBy(x => x.AbpUser.EmailAddress),
                "AbpUser.EmailAddress desc" => query.OrderByDescending(x => x.AbpUser.EmailAddress),
                _ => query.OrderBy(x => x.FullName) // Default sorting
            };

            var patients = await orderedQuery
                .PageBy(input)
                .ToListAsync();

            var mapped = ObjectMapper.Map<List<PatientsForDoctorAndNurseDto>>(patients);
            return new PagedResultDto<PatientsForDoctorAndNurseDto>(totalCount, mapped);
        }


        [HttpGet]
        public async Task<PagedResultDto<PatientsForDoctorAndNurseDto>> PatientsForDoctor(PagedPatientResultRequestDto input)
        {
            long doctorID = 0;
            bool isAdmin = false;

            if (AbpSession.UserId.HasValue)
            {
                var doctor = _doctorAppService.GetDoctorDetailsByAbpUserID(AbpSession.UserId.Value);
                if (doctor != null)
                    doctorID = doctor.Id;
                else
                    isAdmin = true; // ✅ agar doctor null hai → admin (sab records dikhao)
            }

            var query = Repository.GetAll()
                .Include(x => x.AbpUser)
                 .Include(x => x._PatientDischarge)
                .Include(p => p.Admissions)
                    .ThenInclude(a => a.Doctor)
                .Include(p => p.Admissions)
                    .ThenInclude(a => a.Nurse)
                .Where(p => p.IsAdmitted)
                .WhereIf(!isAdmin && doctorID > 0, p => p.Admissions.Any(a => a.DoctorId == doctorID))
                .WhereIf(!input.Keyword.IsNullOrWhiteSpace(),
                    x => x.FullName.Contains(input.Keyword) ||
                         (x.AbpUser != null && x.AbpUser.EmailAddress.Contains(input.Keyword)));

            var totalCount = await query.CountAsync();

            var orderedQuery = input.Sorting switch
            {
                "FullName" => query.OrderBy(x => x.FullName),
                "FullName desc" => query.OrderByDescending(x => x.FullName),
                "AbpUser.EmailAddress" => query.OrderBy(x => x.AbpUser.EmailAddress),
                "AbpUser.EmailAddress desc" => query.OrderByDescending(x => x.AbpUser.EmailAddress),
                _ => query.OrderBy(x => x.FullName)
            };

            var patients = await orderedQuery
                .PageBy(input)
                .ToListAsync();

            var mapped = ObjectMapper.Map<List<PatientsForDoctorAndNurseDto>>(patients);
            return new PagedResultDto<PatientsForDoctorAndNurseDto>(totalCount, mapped);
        }



        [HttpGet]
        public async Task<PatientDetailsAndMedicalHistoryDto> PatientDetailsAndMedicalHistory(long patientId)
        {
            var data = await Repository.GetAll()
                .Include(x => x.AbpUser)
                .Include(x => x.Admissions)
                    .ThenInclude(a => a.Doctor)
                .Include(x => x.Admissions)
                    .ThenInclude(a => a.Nurse)
                .Include(x => x.Prescriptions)
                    .ThenInclude(x => x.Items)
                .Include(x => x.Vitals)
                .Include(x => x.Appointments)
                    .ThenInclude(x => x.Doctor)
                //.Include(x => x.Appointments)
                //    .ThenInclude(x => x.Nurse)
                .FirstOrDefaultAsync(x => x.Id == patientId);

            return ObjectMapper.Map<PatientDetailsAndMedicalHistoryDto>(data);
        }


        [HttpGet]
        public List<PatientDropDownDto> PatientDropDown()
        {
            var userId = AbpSession.UserId;
            if (!userId.HasValue)
                return new List<PatientDropDownDto>();

            var doctor = _doctorAppService.GetDoctorDetailsByAbpUserID(userId.Value);
            var nurse = _nurseAppService.GetNurseDetailsByAbpUserID(userId.Value);

            var query = Repository.GetAll();

            //if (doctor != null)
            //{
            //    query = query.Where(i => i.AssignedDoctorId == doctor.Id);
            //}
            //else if (nurse != null)
            //{
            //    query = query.Where(i => i.AssignedNurseId == nurse.Id);
            //}
             if (AbpSession.TenantId.HasValue)
            {
                query = query.Where(i => i.TenantId == AbpSession.TenantId.Value);
            }

            var patients = query.ToList();
            var mapped = ObjectMapper.Map<List<PatientDropDownDto>>(patients);
            return mapped;
        }
        [HttpGet]
        public List<PatientDropDownDto> GetOpdPatients()
        {
            var tenantId = AbpSession.TenantId;
            if (!tenantId.HasValue)
                return new List<PatientDropDownDto>();

            return Repository.GetAll()
                // tenant filter + OPD-only (not admitted)
                .Where(p => p.TenantId == tenantId.Value && !p.IsAdmitted)
                // optional: scope by doctor/nurse
                // .Where(p => doctor != null    ? p.AssignedDoctorId == doctor.Id 
                //          : nurse  != null    ? p.AssignedNurseId  == nurse.Id 
                //          : true)
                .Select(p => new PatientDropDownDto
                {
                    Id = p.Id,
                    FullName = p.FullName,
                    IsAdmitted = p.IsAdmitted
                })
                .ToList();
        }

        [HttpGet]
        public List<PatientDropDownDto> GetIpdPatients()
        {
            var tenantId = AbpSession.TenantId;
            if (!tenantId.HasValue)
                return new List<PatientDropDownDto>();

            return Repository.GetAll()
                // tenant filter + IPD-only
                .Where(p => p.TenantId == tenantId.Value && p.IsAdmitted)
                // direct projection in SQL
                .Select(p => new PatientDropDownDto
                {
                    Id = p.Id,
                    FullName = p.FullName,
                    IsAdmitted = p.IsAdmitted
                })
                .ToList();
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

        //public async Task<string> CreateWithStripeAsync(CreateUpdatePatientDto input)
        //{
        //    if (input.PaymentMethod == PaymentMethod.Cash)
        //    {
        //        await CreateAsync(input);
        //        return "CASH_SUCCESS";
        //    }

        //    if (!input.DepositAmount.HasValue || input.DepositAmount <= 0)
        //        throw new UserFriendlyException("Deposit amount must be greater than 0");

        //    // ✅ Safely create patient and get ID
        //    var result = await CreateAsync(input);
        //    long patientId = result.Id;

        //    string baseUrl = "http://localhost:4200/app/users";
        //    string successUrl = $"{baseUrl}?payment=success&patientId={patientId}";
        //    string cancelUrl = $"{baseUrl}?payment=cancel&patientId={patientId}";

        //    string stripeUrl = await CreateStripeCheckoutSessionForDeposit(
        //        patientId,
        //        input.DepositAmount.Value,
        //        successUrl,
        //        cancelUrl
        //    );

        //    return stripeUrl;
        //}

        public async Task<string> CreateStripeCheckoutSessionForDeposit(long patientId, long amount, string successUrl, string cancelUrl)
        {
            var options = new SessionCreateOptions
            {
                PaymentMethodTypes = new List<string> { "card" },
                LineItems = new List<SessionLineItemOptions>
        {
            new SessionLineItemOptions
            {
                PriceData = new SessionLineItemPriceDataOptions
                {
                    UnitAmount = (long)(amount * 100),
                    Currency = "inr",
                    ProductData = new SessionLineItemPriceDataProductDataOptions
                    {
                        Name = $"Patient Deposit #{patientId}",
                        Description = "Initial deposit for registration"
                    },
                },
                Quantity = 1,
            }
        },
                Mode = "payment",
                SuccessUrl = successUrl,
                CancelUrl = cancelUrl,
                Metadata = new Dictionary<string, string>
        {
            { "patientId", patientId.ToString() },
            { "purpose", "deposit" }
        }
            };

            var service = new SessionService();
            var session = await service.CreateAsync(options);
            return session.Url;
        }


    }
}
