using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.Extensions;
using Abp.IdentityFramework;
using Abp.Linq.Extensions;
using Abp.MultiTenancy;
using Abp.Runtime.Security;
using EMRSystem.Authorization;
using EMRSystem.Authorization.Roles;
using EMRSystem.Authorization.Users;
using EMRSystem.Editions;
using EMRSystem.MultiTenancy.Dto;
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;

namespace EMRSystem.MultiTenancy;

[AbpAuthorize(PermissionNames.Pages_Tenants)]
public class TenantAppService : AsyncCrudAppService<Tenant, TenantDto, int, PagedTenantResultRequestDto, CreateTenantDto, TenantDto>, ITenantAppService
{
    private readonly TenantManager _tenantManager;
    private readonly EditionManager _editionManager;
    private readonly UserManager _userManager;
    private readonly RoleManager _roleManager;
    private readonly IAbpZeroDbMigrator _abpZeroDbMigrator;
    private readonly IPermissionManager _permissionManager;

    public TenantAppService(
        IRepository<Tenant, int> repository,
        TenantManager tenantManager,
        EditionManager editionManager,
        UserManager userManager,
        RoleManager roleManager,
        IAbpZeroDbMigrator abpZeroDbMigrator,
        IPermissionManager permissionManager)
        : base(repository)
    {
        _tenantManager = tenantManager;
        _editionManager = editionManager;
        _userManager = userManager;
        _roleManager = roleManager;
        _abpZeroDbMigrator = abpZeroDbMigrator;
        _permissionManager = permissionManager;
    }

    //public override async Task<TenantDto> CreateAsync(CreateTenantDto input)
    //{
    //    CheckCreatePermission();

    //    // Create tenant
    //    var tenant = ObjectMapper.Map<Tenant>(input);
    //    tenant.ConnectionString = input.ConnectionString.IsNullOrEmpty()
    //        ? null
    //        : SimpleStringCipher.Instance.Encrypt(input.ConnectionString);

    //    var defaultEdition = await _editionManager.FindByNameAsync(EditionManager.DefaultEditionName);
    //    if (defaultEdition != null)
    //    {
    //        tenant.EditionId = defaultEdition.Id;
    //    }

    //    await _tenantManager.CreateAsync(tenant);
    //    await CurrentUnitOfWork.SaveChangesAsync(); // To get new tenant's id.

    //    // Create tenant database
    //    _abpZeroDbMigrator.CreateOrMigrateForTenant(tenant);

    //    // We are working entities of new tenant, so changing tenant filter
    //    using (CurrentUnitOfWork.SetTenantId(tenant.Id))
    //    {
    //        // Create static roles for new tenant
    //        CheckErrors(await _roleManager.CreateStaticRoles(tenant.Id));

    //        await CurrentUnitOfWork.SaveChangesAsync(); // To get static role ids

    //        // Grant all permissions to admin role
    //        var adminRole = _roleManager.Roles.Single(r => r.Name == StaticRoleNames.Tenants.Admin);
    //        await _roleManager.GrantAllPermissionsAsync(adminRole);

    //        // Create admin user for the tenant
    //        var adminUser = User.CreateTenantAdminUser(tenant.Id, input.AdminEmailAddress);
    //        await _userManager.InitializeOptionsAsync(tenant.Id);
    //        CheckErrors(await _userManager.CreateAsync(adminUser, User.DefaultPassword));
    //        await CurrentUnitOfWork.SaveChangesAsync(); // To get admin user's id

    //        // Assign admin user to role!
    //        CheckErrors(await _userManager.AddToRoleAsync(adminUser, adminRole.Name));
    //        await CurrentUnitOfWork.SaveChangesAsync();

    //        // Extra roles create karna tenant ke liye
    //        string[] roles = { "Billing Staff", "Doctor", "Lab Technician", "Nurse", "Patient", "Pharmacist" };

    //        foreach (var roleName in roles)
    //        {
    //            await CreateRoleIfNotExistsAsync(tenant.Id, roleName);
    //        }

    //    }

    //    return MapToEntityDto(tenant);
    //}

    public override async Task<TenantDto> CreateAsync(CreateTenantDto input)
    {
        CheckCreatePermission();

        // Step 1️⃣: Create Tenant
        var tenant = ObjectMapper.Map<Tenant>(input);
        tenant.ConnectionString = input.ConnectionString.IsNullOrEmpty()
            ? null
            : SimpleStringCipher.Instance.Encrypt(input.ConnectionString);

        var defaultEdition = await _editionManager.FindByNameAsync(EditionManager.DefaultEditionName);
        if (defaultEdition != null)
        {
            tenant.EditionId = defaultEdition.Id;
        }

        await _tenantManager.CreateAsync(tenant);
        await CurrentUnitOfWork.SaveChangesAsync();

        // Step 2️⃣: Create Tenant DB
        _abpZeroDbMigrator.CreateOrMigrateForTenant(tenant);

        using (CurrentUnitOfWork.SetTenantId(tenant.Id))
        {
            // Step 3️⃣: Create static roles
            CheckErrors(await _roleManager.CreateStaticRoles(tenant.Id));
            await CurrentUnitOfWork.SaveChangesAsync();

            // Step 4️⃣: Grant all permissions to admin
            var adminRole = _roleManager.Roles.Single(r => r.Name == StaticRoleNames.Tenants.Admin);
            await _roleManager.GrantAllPermissionsAsync(adminRole);

            // Step 5️⃣: Create admin user
            var adminUser = User.CreateTenantAdminUser(tenant.Id, input.AdminEmailAddress);
            await _userManager.InitializeOptionsAsync(tenant.Id);
            CheckErrors(await _userManager.CreateAsync(adminUser, User.DefaultPassword));
            await CurrentUnitOfWork.SaveChangesAsync();

            CheckErrors(await _userManager.AddToRoleAsync(adminUser, adminRole.Name));
            await CurrentUnitOfWork.SaveChangesAsync();

            // Step 6️⃣: Create Extra Roles
            var rolePermissionsMap = new Dictionary<string, string[]>
                {
                    { "Doctor", new[]
                        {
                            "Pages.Doctor",
                            "Pages.Doctor.Appointments.View",
                            "Pages.Doctor.Patients.View",
                            "Pages.Doctor.Prescriptions.Manage",
                            "Pages.Doctor.LabOrders.Manage",
                            "Pages.Doctor.ConsultationRequests.Manage"
                        }
                    },
                    { "Nurse", new[]
                        {
                            "Pages.Nurse",
                            "Pages.Nurse.Appointments.Manage",
                            "Pages.Nurse.Patients.View",
                            "Pages.Nurse.Vitals.Manage",
                            "Pages.Nurse.Medication.Manage"
                        }
                    },
                    { "Lab Technician", new[]
                        {
                            "Pages.Lab",
                            "Pages.Lab.TestRequests.Manage",
                            "Pages.Lab.Receipts.View"
                        }
                    },
                    { "Pharmacist", new[]
                        {
                            "Pages.Pharmacy",
                            "Pages.Pharmacy.MedicineList.View",
                            "Pages.Pharmacy.Purchase.Manage",
                            "Pages.Pharmacy.Stock.View",
                            "Pages.Pharmacy.Prescriptions.View"
                        }
                    },
                    { "Billing Staff", new[]
                        {
                            "Pages.Billing",
                            "Pages.Billing.InsuranceClaims.View",
                            "Pages.Billing.Invoices.View",
                            "Pages.Billing.Deposits.Manage"
                        }
                    },
                    { "Patient", new[]
                        {
                            "Pages.Patient",
                            "Pages.Patient.Appointments.View"
                        }
                    }
                };

            foreach (var kvp in rolePermissionsMap)
            {
                var roleName = kvp.Key;
                var permissions = kvp.Value;

                var role = await CreateRoleIfNotExistsAsync(tenant.Id, roleName);

                foreach (var permName in permissions)
                {
                    var permission = _permissionManager.GetPermissionOrNull(permName);
                    if (permission != null)
                    {
                        await _roleManager.GrantPermissionAsync(role, permission);
                    }
                }
            }

            await CurrentUnitOfWork.SaveChangesAsync();
        }

        return MapToEntityDto(tenant);
    }

    protected override IQueryable<Tenant> CreateFilteredQuery(PagedTenantResultRequestDto input)
    {
        return Repository.GetAll()
            .WhereIf(!input.Keyword.IsNullOrWhiteSpace(), x => x.TenancyName.Contains(input.Keyword) || x.Name.Contains(input.Keyword))
            .WhereIf(input.IsActive.HasValue, x => x.IsActive == input.IsActive);
    }

    protected override IQueryable<Tenant> ApplySorting(IQueryable<Tenant> query, PagedTenantResultRequestDto input)
    {
        return query.OrderBy(input.Sorting);
    }

    protected override void MapToEntity(TenantDto updateInput, Tenant entity)
    {
        // Manually mapped since TenantDto contains non-editable properties too.
        entity.Name = updateInput.Name;
        entity.TenancyName = updateInput.TenancyName;
        entity.IsActive = updateInput.IsActive;
    }

    public override async Task DeleteAsync(EntityDto<int> input)
    {
        CheckDeletePermission();

        var tenant = await _tenantManager.GetByIdAsync(input.Id);
        await _tenantManager.DeleteAsync(tenant);
    }

    private void CheckErrors(IdentityResult identityResult)
    {
        identityResult.CheckErrors(LocalizationManager);
    }
    private async Task<Role> CreateRoleIfNotExistsAsync(int tenantId, string roleName)
    {
        var existing = _roleManager.Roles.FirstOrDefault(r => r.TenantId == tenantId && r.Name == roleName);
        if (existing != null)
        {
            return existing;
        }

        var newRole = new Role(tenantId, roleName, roleName)
        {
            IsStatic = false,
            IsDefault = false
        };
        CheckErrors(await _roleManager.CreateAsync(newRole));
        return newRole;
    }
}

