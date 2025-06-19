using EMRSystem.Nurses;
using EMRSystem.Patients;

namespace EMRSystem.Authorization;

public static class PermissionNames
{
    public const string Pages_Tenants = "Pages.Tenants";
    public const string Pages_Users = "Pages.Users";
    public const string Pages_Users_Activation = "Pages.Users.Activation";
    public const string Pages_Roles = "Pages.Roles";
    public const string Pages_Tenant_ManagePrescriptions = "Pages.Tenant.ManagePrescriptions";
    public const string Pages_Tenant_ManageAppointments = "Pages.Tenant.ManageAppointments";
    public const string Pages_Tenant_ManageVisits = "Pages.Tenant.ManageVisits";

    public const string Pages_Doctors = "Pages.Doctors";
    public const string Pages_Billing = "Pages.BillingStaff";
    public const string Pages_LabReports = "Pages.LabTechnician";
    public const string Pages_Nurses = "Pages.Nurse";
    //public const string Pages_Patients = "Pages.Patient";
    public const string Pages_Patients = "Pages.Doctors.Patients";

    public const string Pages_Pharmacist = "Pages.Pharmacist";
    //public const string Pages_Appointments = "Pages.Appointments";
    //public const string Pages_Prescriptions = "Pages.Prescriptions";
}
