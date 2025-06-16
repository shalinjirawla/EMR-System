using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Domain.Repositories;
using EMRSystem.Authorization.Users;
using EMRSystem.Patients.Dto;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EMRSystem.Patients
{
    [AbpAuthorize("Pages.Doctors.Patients")]

    public class PatientAppService : AsyncCrudAppService<Patient, PatientDto, long, PagedAndSortedResultRequestDto, CreateUpdatePatientDto, CreateUpdatePatientDto>,
    IPatientAppService
    {
        private readonly UserManager _userManager;
        public PatientAppService(IRepository<Patient, long> repository, UserManager userManager) : base(repository)
        {
            _userManager = userManager;
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
    }
}
