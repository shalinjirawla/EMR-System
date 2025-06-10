using Abp.Application.Services.Dto;
using Abp.Application.Services;
using Abp.Domain.Repositories;
using EMRSystem.Billings;
using EMRSystem.BillingStaff.Dto;
using EMRSystem.BillingStaff;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EMRSystem.Doctors;
using EMRSystem.Doctor.Dto;
using Abp.Authorization;
using EMRSystem.Authorization;
using EMRSystem.Authorization.Users;
using EMRSystem.Users.Dto;
using EMRSystem.Users;
using Abp.UI;
using Microsoft.AspNetCore.Identity;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Net.Mail;
using Castle.Core.Resource;

namespace EMRSystem.Doctor
{
    [AbpAuthorize(PermissionNames.Pages_Doctors)]
    public class DoctorAppService : AsyncCrudAppService<EMRSystem.Doctors.Doctor, DoctorDto, long, PagedAndSortedResultRequestDto, CreateUpdateDoctorDto, CreateUpdateDoctorDto>,
   IDoctorAppService
    {
        private readonly IUserAppService _userAppService;
        public DoctorAppService(IRepository<EMRSystem.Doctors.Doctor, long> repository, IUserAppService userAppService) : base(repository)
        {
            _userAppService = userAppService;
        }

        public override async Task<DoctorDto> CreateAsync(CreateUpdateDoctorDto input)
        {
            var abpUser = await _userAppService.CreateAsync(input.AbpUser);

            var doctor = ObjectMapper.Map<EMRSystem.Doctors.Doctor>(input);
            doctor.FullName = input.AbpUser.Name + " " + input.AbpUser.Surname;
            doctor.AbpUserId = abpUser.Id;

            var result = await Repository.InsertAsync(doctor);
            var mappedResult = ObjectMapper.Map<DoctorDto>(result);
            return mappedResult;
        }
    }
}
