using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Domain.Repositories;
using EMRSystem.Authorization.Users;
using EMRSystem.Nurse;
using EMRSystem.Nurse.Dto;
using EMRSystem.Patients.Dto;
using EMRSystem.Prescriptions;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Abp.Collections.Extensions;
using EMRSystem.Appointments.Dto;
using EMRSystem.Appointments;
using Microsoft.AspNetCore.Mvc;
using EMRSystem.Doctor;
using Abp.Linq.Extensions;
using System.Linq.Dynamic.Core;
namespace EMRSystem.Patients
{
    //[AbpAuthorize("Pages.Doctors.Patients")]

    public class PatientAppService : AsyncCrudAppService<Patient, PatientDto, long, PagedAndSortedResultRequestDto, CreateUpdatePatientDto, CreateUpdatePatientDto>,
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

        protected override IQueryable<Patient> CreateFilteredQuery(PagedAndSortedResultRequestDto input)
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
        public async Task<PagedResultDto<PatientsForDoctorAndNurseDto>> PatientsForNurse(PagedAndSortedResultRequestDto input)
        {

            long nurseID = 0;
            if (AbpSession.UserId.HasValue)
            {
                var nurse = _nurseAppService.GetNurseDetailsByAbpUserID(AbpSession.UserId.Value);
                if (nurse != null)
                    nurseID = nurse.Id;
            }


            var patients = Repository.GetAll().Include(x => x.Doctors).Include(x => x.AbpUser)
                        .WhereIf(nurseID > 0, i => i.AssignedNurseId == nurseID);

            var patientList = patients.ToList();
            var totalCount = patientList.Count();

            var query =await patients
                    .OrderBy(input.Sorting ?? "Id DESC")
                    .PageBy(input)
                    .ToListAsync();

            var mapped = ObjectMapper.Map<List<PatientsForDoctorAndNurseDto>>(patients);
            return new PagedResultDto<PatientsForDoctorAndNurseDto>(
                 totalCount,
                 mapped
             );
        }

        [HttpGet]
        public async Task<PagedResultDto<PatientsForDoctorAndNurseDto>> PatientsForDoctor(PagedAndSortedResultRequestDto input)
        {

            long doctorID = 0;
            if (AbpSession.UserId.HasValue)
            {
                var doctor = _doctorAppService.GetDoctorDetailsByAbpUserID(AbpSession.UserId.Value);
                if (doctor != null)
                    doctorID = doctor.Id;
            }


            var patients = Repository.GetAll().Include(x => x.Nurses).Include(x => x.AbpUser)
                        .WhereIf(doctorID > 0, i => i.AssignedDoctorId == doctorID);

            var patientList = patients.ToList();
            var totalCount = patientList.Count();

            var query = await patients
                   .OrderBy(input.Sorting ?? "Id DESC")
                   .PageBy(input)
                   .ToListAsync();

            var mapped = ObjectMapper.Map<List<PatientsForDoctorAndNurseDto>>(patients);
            return new PagedResultDto<PatientsForDoctorAndNurseDto>(
                 totalCount,
                 mapped
             );
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
