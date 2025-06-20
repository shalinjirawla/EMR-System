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

namespace EMRSystem.Patients
{
    //[AbpAuthorize("Pages.Doctors.Patients")]

    public class PatientAppService : AsyncCrudAppService<Patient, PatientDto, long, PagedAndSortedResultRequestDto, CreateUpdatePatientDto, CreateUpdatePatientDto>,
    IPatientAppService
    {
        private readonly UserManager _userManager;
        private readonly INurseAppService _nurseAppService;
        public PatientAppService(IRepository<Patient, long> repository, UserManager userManager, INurseAppService nurseAppService) : base(repository)
        {
            _userManager = userManager;
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

        public IQueryable<Patient> GetAllAssignedPatientForNurse(PagedAndSortedResultRequestDto input)
        {
            //long nurseID = 0;
            //if (AbpSession.UserId.HasValue)
            //{
            //    var nurse = _nurseAppService.GetNurseDetailsByAbpUserID(AbpSession.UserId.Value);
            //    if (nurse != null)
            //        nurseID = nurse.Id;
            //}

            var patients = Repository.GetAll()
                            .Include(x => x.Nurses)
                            .Include(x => x.Doctors)
                            .Include(x => x.AbpUser)
                            .Select(x => new Patient
                            {
                                Id = x.Id,
                                TenantId = x.TenantId,
                                FullName = x.FullName,
                                Gender = x.Gender,
                                DateOfBirth = x.DateOfBirth,
                                Address = x.Address,
                                BloodGroup = x.BloodGroup,
                                EmergencyContactName = x.EmergencyContactName,
                                EmergencyContactNumber = x.EmergencyContactNumber,
                                IsAdmitted = x.IsAdmitted,
                                AdmissionDate = x.AdmissionDate,
                                DischargeDate = x.DischargeDate,
                                InsuranceProvider = x.InsuranceProvider,
                                InsurancePolicyNumber = x.InsurancePolicyNumber,
                                AssignedNurseId = x.AssignedNurseId,
                                AssignedDoctorId = x.AssignedDoctorId,
                                AbpUserId = x.AbpUserId,
                                Nurses = x.Nurses == null ? null : new EMRSystem.Nurses.Nurse
                                {
                                    Id = x.Nurses.Id,
                                    FullName = x.Nurses.FullName
                                },
                                Doctors = x.Doctors == null ? null : new EMRSystem.Doctors.Doctor
                                {
                                    Id = x.Doctors.Id,
                                    FullName = x.Doctors.FullName
                                },
                                AbpUser = x.AbpUser == null ? null : new EMRSystem.Authorization.Users.User
                                {
                                    Id = x.AbpUser.Id
                                }
                            });
            return patients;
        }
    }
}
