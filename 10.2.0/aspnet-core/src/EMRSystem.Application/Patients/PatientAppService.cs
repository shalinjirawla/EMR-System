using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Collections.Extensions;
using Abp.Domain.Repositories;
using Abp.Extensions;
using Abp.Linq.Extensions;
using EMRSystem.Appointments;
using EMRSystem.Appointments.Dto;
using EMRSystem.Authorization.Users;
using EMRSystem.Doctor;
using EMRSystem.Nurse;
using EMRSystem.Nurse.Dto;
using EMRSystem.Nurses;
using EMRSystem.Patients.Dto;
using EMRSystem.Prescriptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
        public PatientAppService(IRepository<Patient, long> repository, UserManager userManager, IDoctorAppService doctorAppService, INurseAppService nurseAppService) : base(repository)
        {
            _userManager = userManager;
            _doctorAppService = doctorAppService;
            _nurseAppService = nurseAppService;
        }

        protected override IQueryable<Patient> CreateFilteredQuery(PagedPatientResultRequestDto input)
        {
            return Repository.GetAll();
        }

        public override async Task<PatientDto> CreateAsync(CreateUpdatePatientDto input)
        {
            CheckCreatePermission();
            var user = ObjectMapper.Map<Patient>(input);
            await _userManager.InitializeOptionsAsync(AbpSession.TenantId);
            await Repository.InsertAsync(user);
            CurrentUnitOfWork.SaveChanges();
            return MapToEntityDto(user);
        }

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

        public async Task<ListResultDto<PatientDto>> GetAllPatientByTenantID(int tenantId)
        {
            var patientsData = await Repository.GetAllAsync();
            var patients = patientsData.Where(x => x.TenantId == tenantId).ToList();

            var mapped = ObjectMapper.Map<List<PatientDto>>(patients);
            var resultList = new ListResultDto<PatientDto>(mapped);
            return resultList;
        }

        [HttpGet]
        public async Task<PagedResultDto<PatientsForDoctorAndNurseDto>> PatientsForNurse(PagedPatientResultRequestDto input)
        {
            long nurseID = 0;
            if (AbpSession.UserId.HasValue)
            {
                var nurse = _nurseAppService.GetNurseDetailsByAbpUserID(AbpSession.UserId.Value);
                if (nurse != null)
                    nurseID = nurse.Id;
            }

            var query = Repository.GetAll()
                .Include(x => x.Doctors)
                .Include(x => x.AbpUser)
                .WhereIf(nurseID > 0, i => i.AssignedNurseId == nurseID)
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
            if (AbpSession.UserId.HasValue)
            {
                var doctor = _doctorAppService.GetDoctorDetailsByAbpUserID(AbpSession.UserId.Value);
                if (doctor != null)
                    doctorID = doctor.Id;
            }


            var query = Repository.GetAll()
                .Include(x => x.Nurses)
                .Include(x => x.AbpUser)
                .WhereIf(doctorID > 0, i => i.AssignedDoctorId == doctorID)
                .WhereIf(!input.Keyword.IsNullOrWhiteSpace(),
                    x=>x.FullName.Contains(input.Keyword)||
                     (x.AbpUser != null && x.AbpUser.EmailAddress.Contains(input.Keyword)));

            var totalCount = await query.CountAsync();

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
            return new PagedResultDto<PatientsForDoctorAndNurseDto>(totalCount,mapped);
        }

        [HttpGet]
        public async Task<PatientDetailsAndMedicalHistoryDto> PatientDetailsAndMedicalHistory(long patientId)
        {
            var data = await Repository.GetAll()
                            .Include(x => x.AbpUser)
                            .Include(x => x.Nurses)
                            .Include(x => x.Doctors)
                            .Include(x => x.Prescriptions)
                            .ThenInclude(x => x.Items)
                            .Include(x => x.Vitals)
                            .Include(x => x.Appointments).ThenInclude(x => x.Doctor)
                            .Include(x => x.Appointments).ThenInclude(x => x.Nurse)
                            .FirstOrDefaultAsync(x => x.Id == patientId);

            var result = ObjectMapper.Map<PatientDetailsAndMedicalHistoryDto>(data);
            return result;

        }

        public List<PatientDropDownDto> PatientDropDown()
        {
            var userId = AbpSession.UserId;
            if (!userId.HasValue)
                return new List<PatientDropDownDto>();

            var doctor = _doctorAppService.GetDoctorDetailsByAbpUserID(userId.Value);
            var nurse = _nurseAppService.GetNurseDetailsByAbpUserID(userId.Value);

            var query = Repository.GetAll();

            if (doctor != null)
            {
                query = query.Where(i => i.AssignedDoctorId == doctor.Id);
            }
            else if (nurse != null)
            {
                query = query.Where(i => i.AssignedNurseId == nurse.Id);
            }
            else if (AbpSession.TenantId.HasValue)
            {
                query = query.Where(i => i.TenantId == AbpSession.TenantId.Value);
            }

            var patients = query.ToList();
            var mapped = ObjectMapper.Map<List<PatientDropDownDto>>(patients);
            return mapped;
        }
    }
}
