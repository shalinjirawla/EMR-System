using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Domain.Repositories;
using EMRSystem.Insurances.Dto;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMRSystem.Insurances
{
    public class PatientInsuranceAppService :
        AsyncCrudAppService<PatientInsurance, PatientInsuranceDto, long,
            PagedAndSortedResultRequestDto, CreateUpdatePatientInsuranceDto, CreateUpdatePatientInsuranceDto>,
        IPatientInsuranceAppService
    {
        private readonly IRepository<InsuranceMaster, long> _insuranceRepository;

        public PatientInsuranceAppService(IRepository<PatientInsurance, long> repository,
            IRepository<InsuranceMaster, long> insuranceRepository) : base(repository)
        {
            _insuranceRepository = insuranceRepository;
        }

        public override async Task<PatientInsuranceDto> GetAsync(EntityDto<long> input)
        {
            var data = await Repository.GetAll()
                .Where(x => x.Id == input.Id)
                .Select(x => new PatientInsuranceDto
                {
                    Id = x.Id,
                    TenantId = x.TenantId,
                    PatientId = x.PatientId,
                    InsuranceId = x.InsuranceId,
                    PolicyNumber = x.PolicyNumber,
                    CoverageLimit = x.CoverageLimit,
                    CoPayPercentage = x.CoPayPercentage,
                    IsActive = x.IsActive,
                    InsuranceName = _insuranceRepository.FirstOrDefault(y => y.Id == x.InsuranceId).InsuranceName
                }).FirstOrDefaultAsync();

            return data;
        }
    }
}
