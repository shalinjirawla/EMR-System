using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Domain.Entities;
using Abp.Domain.Repositories;
using Abp.Extensions;
using Abp.IdentityFramework;
using Abp.Linq.Extensions;
using Abp.Localization;
using Abp.Runtime.Session;
using Abp.UI;
using EMRSystem.Authorization;
using EMRSystem.Authorization.Roles;
using EMRSystem.Authorization.Users;
using EMRSystem.Roles.Dto;
using EMRSystem.Users.Dto;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;

namespace EMRSystem.Users;

[AbpAuthorize(PermissionNames.Pages_Users)]
public class UserAppService : AsyncCrudAppService<User, UserDto, long, PagedUserResultRequestDto, CreateUserDto, UserDto>, IUserAppService
{
    private readonly UserManager _userManager;
    private readonly RoleManager _roleManager;
    private readonly IRepository<Role> _roleRepository;
    private readonly IPasswordHasher<User> _passwordHasher;
    private readonly IAbpSession _abpSession;
    private readonly LogInManager _logInManager;

    public UserAppService(
        IRepository<User, long> repository,
        UserManager userManager,
        RoleManager roleManager,
        IRepository<Role> roleRepository,
        IPasswordHasher<User> passwordHasher,
        IAbpSession abpSession,
        LogInManager logInManager)
        : base(repository)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _roleRepository = roleRepository;
        _passwordHasher = passwordHasher;
        _abpSession = abpSession;
        _logInManager = logInManager;
    }

    public override async Task<UserDto> CreateAsync(CreateUserDto input)
    {
        CheckCreatePermission();

        var user = ObjectMapper.Map<User>(input);

        user.TenantId = AbpSession.TenantId;
        user.IsEmailConfirmed = true;

        await _userManager.InitializeOptionsAsync(AbpSession.TenantId);

        CheckErrors(await _userManager.CreateAsync(user, input.Password));

        if (input.RoleNames != null)
        {
            CheckErrors(await _userManager.SetRolesAsync(user, input.RoleNames));
        }

        CurrentUnitOfWork.SaveChanges();

        return MapToEntityDto(user);
    }

    public override async Task<UserDto> UpdateAsync(UserDto input)
    {
        CheckUpdatePermission();

        var user = await _userManager.GetUserByIdAsync(input.Id);

        MapToEntity(input, user);

        CheckErrors(await _userManager.UpdateAsync(user));

        if (input.RoleNames != null)
        {
            CheckErrors(await _userManager.SetRolesAsync(user, input.RoleNames));
        }

        return await GetAsync(input);
    }

    public override async Task DeleteAsync(EntityDto<long> input)
    {
        var user = await _userManager.GetUserByIdAsync(input.Id);
        await _userManager.DeleteAsync(user);
    }

    [AbpAuthorize(PermissionNames.Pages_Users_Activation)]
    public async Task Activate(EntityDto<long> user)
    {
        await Repository.UpdateAsync(user.Id, async (entity) =>
        {
            entity.IsActive = true;
        });
    }

    [AbpAuthorize(PermissionNames.Pages_Users_Activation)]
    public async Task DeActivate(EntityDto<long> user)
    {
        await Repository.UpdateAsync(user.Id, async (entity) =>
        {
            entity.IsActive = false;
        });
    }

    public async Task<ListResultDto<RoleDto>> GetRoles()
    {
        var roles = await _roleRepository.GetAllListAsync();
        return new ListResultDto<RoleDto>(ObjectMapper.Map<List<RoleDto>>(roles));
    }

    public async Task ChangeLanguage(ChangeUserLanguageDto input)
    {
        await SettingManager.ChangeSettingForUserAsync(
            AbpSession.ToUserIdentifier(),
            LocalizationSettingNames.DefaultLanguage,
            input.LanguageName
        );
    }

    protected override User MapToEntity(CreateUserDto createInput)
    {
        var user = ObjectMapper.Map<User>(createInput);
        user.SetNormalizedNames();
        return user;
    }

    protected override void MapToEntity(UserDto input, User user)
    {
        ObjectMapper.Map(input, user);
        user.SetNormalizedNames();
    }

    protected override UserDto MapToEntityDto(User user)
    {
        var roleIds = user.Roles.Select(x => x.RoleId).ToArray();

        var roles = _roleManager.Roles.Where(r => roleIds.Contains(r.Id)).Select(r => r.NormalizedName);

        var userDto = base.MapToEntityDto(user);
        userDto.RoleNames = roles.ToArray();

        return userDto;
    }

    protected override IQueryable<User> CreateFilteredQuery(PagedUserResultRequestDto input)
    {
        return Repository.GetAllIncluding(x => x.Roles)
            .WhereIf(!input.Keyword.IsNullOrWhiteSpace(), x => x.UserName.Contains(input.Keyword) || x.Name.Contains(input.Keyword) || x.EmailAddress.Contains(input.Keyword))
            .WhereIf(input.IsActive.HasValue, x => x.IsActive == input.IsActive);
    }

    protected override async Task<User> GetEntityByIdAsync(long id)
    {
        var user = await Repository.GetAllIncluding(x => x.Roles).FirstOrDefaultAsync(x => x.Id == id);

        if (user == null)
        {
            throw new EntityNotFoundException(typeof(User), id);
        }

        return user;
    }

    protected override IQueryable<User> ApplySorting(IQueryable<User> query, PagedUserResultRequestDto input)
    {
        return query.OrderBy(input.Sorting);
    }

    protected virtual void CheckErrors(IdentityResult identityResult)
    {
        identityResult.CheckErrors(LocalizationManager);
    }

    public async Task<bool> ChangePassword(ChangePasswordDto input)
    {
        await _userManager.InitializeOptionsAsync(AbpSession.TenantId);

        var user = await _userManager.FindByIdAsync(AbpSession.GetUserId().ToString());
        if (user == null)
        {
            throw new Exception("There is no current user!");
        }

        if (await _userManager.CheckPasswordAsync(user, input.CurrentPassword))
        {
            CheckErrors(await _userManager.ChangePasswordAsync(user, input.NewPassword));
        }
        else
        {
            CheckErrors(IdentityResult.Failed(new IdentityError
            {
                Description = "Incorrect password."
            }));
        }

        return true;
    }

    public async Task<bool> ResetPassword(ResetPasswordDto input)
    {
        if (_abpSession.UserId == null)
        {
            throw new UserFriendlyException("Please log in before attempting to reset password.");
        }

        var currentUser = await _userManager.GetUserByIdAsync(_abpSession.GetUserId());
        var loginAsync = await _logInManager.LoginAsync(currentUser.UserName, input.AdminPassword, shouldLockout: false);
        if (loginAsync.Result != AbpLoginResultType.Success)
        {
            throw new UserFriendlyException("Your 'Admin Password' did not match the one on record.  Please try again.");
        }

        if (currentUser.IsDeleted || !currentUser.IsActive)
        {
            return false;
        }

        var roles = await _userManager.GetRolesAsync(currentUser);
        if (!roles.Contains(StaticRoleNames.Tenants.Admin))
        {
            throw new UserFriendlyException("Only administrators may reset passwords.");
        }

        var user = await _userManager.GetUserByIdAsync(input.UserId);
        if (user != null)
        {
            user.Password = _passwordHasher.HashPassword(user, input.NewPassword);
            await CurrentUnitOfWork.SaveChangesAsync();
        }

        return true;
    }

    public async Task<User> GetUserDetailsById(long id)
    {
        var details = await Repository.GetAllIncludingAsync(x => x.Patients, x => x.Doctors, x => x.Nurses, x => x.LabTechnicians).Result
             .Select(x => new User
             {
                 Id = x.Id,
                 TenantId = x.TenantId,
                 UserName = x.UserName,
                 Name = x.Name,
                 Surname = x.Surname,
                 EmailAddress = x.EmailAddress,
                 Roles = x.Roles,
                 Patients = x.Patients.Select(i => new EMRSystem.Patients.Patient
                 {
                     Id = i.Id,
                     FullName = i.FullName,
                     DateOfBirth = i.DateOfBirth,
                     Gender = i.Gender,
                     Address = i.Address,
                     BloodGroup = i.BloodGroup,
                     EmergencyContactName = i.EmergencyContactName,
                     EmergencyContactNumber = i.EmergencyContactNumber,
                     AssignedNurseId = i.AssignedNurseId,
                     IsAdmitted = i.IsAdmitted,
                     AdmissionDate = i.AdmissionDate,
                     DischargeDate = i.DischargeDate,
                     InsuranceProvider = i.InsuranceProvider,
                     InsurancePolicyNumber = i.InsurancePolicyNumber,
                     AbpUserId = i.AbpUserId,
                     AssignedDoctorId = i.AssignedDoctorId,
                 }).ToList(),
                 Nurses = x.Nurses.Select(i => new EMRSystem.Nurses.Nurse
                 {
                     Id = i.Id,
                     FullName = i.FullName,
                     Gender = i.Gender,
                     ShiftTiming = i.ShiftTiming,
                     Department = i.Department,
                     Qualification = i.Qualification,
                     YearsOfExperience = i.YearsOfExperience,
                     DateOfBirth = i.DateOfBirth,
                     AbpUserId = i.AbpUserId,
                 }).ToList(),
                 Doctors = x.Doctors.Select(i => new EMRSystem.Doctors.Doctor
                 {
                     Id = i.Id,
                     FullName = i.FullName,
                     Gender = i.Gender,
                     Specialization = i.Specialization,
                     Qualification = i.Qualification,
                     YearsOfExperience = i.YearsOfExperience,
                     Department = i.Department,
                     RegistrationNumber = i.RegistrationNumber,
                     DateOfBirth = i.DateOfBirth,
                     AbpUserId = i.AbpUserId,
                 }).ToList(),
                 LabTechnicians = x.LabTechnicians.Select(i => new EMRSystem.LabReports.LabTechnician
                 {
                     Id = i.Id,
                     FullName = i.FullName,
                     Gender = i.Gender,
                     Qualification = i.Qualification,
                     YearsOfExperience = i.YearsOfExperience,
                     Department = i.Department,
                     CertificationNumber = i.CertificationNumber,
                     DateOfBirth = i.DateOfBirth,
                     AbpUserId = i.AbpUserId,
                 }).ToList(),
                 //Pharmacists = x.Pharmacists.Select(i => new EMRSystem.Pharmacists.Pharmacist
                 //{
                 //    Id = i.Id,
                 //    FullName = i.FullName,
                 //    Gender = i.Gender,
                 //    Qualification = i.Qualification,
                 //    DateOfBirth = i.DateOfBirth,
                 //    LicenseNumber = i.LicenseNumber,
                 //    LicenseExpiryDate = i.LicenseExpiryDate,
                 //    AbpUserId = i.AbpUserId,
                 //}).ToList(),
                 //Bills = x.Bills.Select(i => new EMRSystem.Billings.Bill
                 //{
                 //    Id = i.Id,
                 //    PatientId = i.PatientId,
                 //    BillDate = i.BillDate,
                 //    AdmissionDate = i.AdmissionDate,
                 //    DateOfSurgery = i.DateOfSurgery,
                 //    TotalAmount = i.TotalAmount,
                 //    PaymentStatus = i.PaymentStatus,
                 //    PaymentMethod = i.PaymentMethod,
                 //    AbpUserId = i.AbpUserId,
                 //}).ToList(),
             })
            .FirstOrDefaultAsync(x => x.Id == id);
        return details;
    }

}

