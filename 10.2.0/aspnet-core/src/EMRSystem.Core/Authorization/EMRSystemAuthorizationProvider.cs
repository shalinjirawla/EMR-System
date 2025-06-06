using Abp.Authorization;
using Abp.Localization;
using Abp.MultiTenancy;

namespace EMRSystem.Authorization;

public class EMRSystemAuthorizationProvider : AuthorizationProvider
{
    public override void SetPermissions(IPermissionDefinitionContext context)
    {
        context.CreatePermission(PermissionNames.Pages_Users, L("Users"));
        context.CreatePermission(PermissionNames.Pages_Users_Activation, L("UsersActivation"));
        context.CreatePermission(PermissionNames.Pages_Roles, L("Roles"));
        context.CreatePermission(PermissionNames.Pages_Tenants, L("Tenants"), multiTenancySides: MultiTenancySides.Host);
        context.CreatePermission(PermissionNames.Pages_Tenant_ManagePrescriptions, L("ManagePrescriptions"));
        context.CreatePermission(PermissionNames.Pages_Tenant_ManageAppointments, L("ManageAppointments"));
        context.CreatePermission(PermissionNames.Pages_Tenant_ManageVisits, L("ManageVisits"));

        context.CreatePermission(PermissionNames.Pages_Doctors, L("Doctors"));
        context.CreatePermission(PermissionNames.Pages_Billing, L("BillingStaff"));
        context.CreatePermission(PermissionNames.Pages_LabReports, L("LabTechnician"));
        context.CreatePermission(PermissionNames.Pages_Nurses, L("Nurse"));
        context.CreatePermission(PermissionNames.Pages_Patients, L("Patient"));
        context.CreatePermission(PermissionNames.Pages_Pharmacist, L("Pharmacist"));
    }

    private static ILocalizableString L(string name)
    {
        return new LocalizableString(name, EMRSystemConsts.LocalizationSourceName);
    }
}
