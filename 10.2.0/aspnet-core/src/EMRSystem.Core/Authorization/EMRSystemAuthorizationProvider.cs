using Abp.Authorization;
using Abp.Localization;
using Abp.MultiTenancy;

namespace EMRSystem.Authorization;

public class EMRSystemAuthorizationProvider : AuthorizationProvider
{
    public override void SetPermissions(IPermissionDefinitionContext context)
    {
        var pages = context.CreatePermission("Pages", L("Pages"));

        #region Admin
        context.CreatePermission(PermissionNames.Pages_Users, L("Users"));
        context.CreatePermission(PermissionNames.Pages_Users_Activation, L("UsersActivation"));
        context.CreatePermission(PermissionNames.Pages_Roles, L("Roles"));
        context.CreatePermission(PermissionNames.Pages_Tenants, L("Tenants"), multiTenancySides: MultiTenancySides.Host);

        #endregion

        #region Doctor Permissions
        var doctors = pages.CreateChildPermission("Pages.Doctors", L("Doctors"));

        var appointments = doctors.CreateChildPermission("Pages.Doctors.Appointments", L("Appointments"));
        appointments.CreateChildPermission("Pages.Doctors.Appointments.View", L("ViewAppointments"));
        appointments.CreateChildPermission("Pages.Doctors.Appointments.StatusUpdate", L("UpdateAppointmentStatus"));
        appointments.CreateChildPermission("Pages.Doctors.Appointments.Schedule", L("ScheduleAppointments"));

        var patients = doctors.CreateChildPermission("Pages.Doctors.Patients", L("Patients"));
        patients.CreateChildPermission("Pages.Doctors.Patients.View", L("ViewPatients"));
        patients.CreateChildPermission("Pages.Doctors.Patients.History", L("ViewPatientHistory"));

        var prescriptions = doctors.CreateChildPermission("Pages.Doctors.Prescriptions", L("Prescriptions"));
        prescriptions.CreateChildPermission("Pages.Doctors.Prescriptions.Create", L("CreatePrescriptions"));
        prescriptions.CreateChildPermission("Pages.Doctors.Prescriptions.Update", L("UpdatePrescriptions"));

        var visits = doctors.CreateChildPermission("Pages.Doctors.Visits", L("Visits"));
        visits.CreateChildPermission("Pages.Doctors.Visits.View", L("ViewVisits"));
        visits.CreateChildPermission("Pages.Doctors.Visits.Complete", L("CompleteVisit"));

        var labOrders = doctors.CreateChildPermission("Pages.Doctors.LabOrders", L("LabOrders"));
        labOrders.CreateChildPermission("Pages.Doctors.LabOrders.Create", L("CreateLabOrders"));
        labOrders.CreateChildPermission("Pages.Doctors.LabOrders.View", L("ViewLabOrders"));
        #endregion

        #region Nurse Permissions
        var nurse = pages.CreateChildPermission("Pages.Nurse", L("Nurse"));

        var assignedPatients = nurse.CreateChildPermission("Pages.Nurse.Patients", L("AssignedPatients"));
        assignedPatients.CreateChildPermission("Pages.Nurse.Patients.View", L("ViewAssignedPatients"));

        var medication = nurse.CreateChildPermission("Pages.Nurse.Medication", L("MedicationLog"));
        medication.CreateChildPermission("Pages.Nurse.Medication.View", L("ViewMedicationLog"));
        medication.CreateChildPermission("Pages.Nurse.Medication.Update", L("UpdateMedicationLog"));

        var vitals = nurse.CreateChildPermission("Pages.Nurse.Vitals", L("VitalsNotes"));
        vitals.CreateChildPermission("Pages.Nurse.Vitals.View", L("ViewVitals"));
        vitals.CreateChildPermission("Pages.Nurse.Vitals.Create", L("CreateVitals"));
        vitals.CreateChildPermission("Pages.Nurse.Vitals.Update", L("UpdateVitals"));
        #endregion

        #region Billing Staff Permissions
        var billing = pages.CreateChildPermission("Pages.BillingStaff", L("BillingStaff"));

        billing.CreateChildPermission("Pages.BillingStaff.Invoices.Create", L("CreateInvoices"));
        billing.CreateChildPermission("Pages.BillingStaff.Invoices.View", L("ViewInvoices"));
        billing.CreateChildPermission("Pages.BillingStaff.Invoices.Update", L("UpdateInvoices"));

        billing.CreateChildPermission("Pages.BillingStaff.Payments.Create", L("CreatePayments"));
        billing.CreateChildPermission("Pages.BillingStaff.Payments.View", L("ViewPayments"));

        billing.CreateChildPermission("Pages.BillingStaff.Insurance.View", L("ViewInsurance"));
        billing.CreateChildPermission("Pages.BillingStaff.Reports.View", L("ViewReports"));
        #endregion

        #region Lab Technician Permissions
        var labTech = pages.CreateChildPermission("Pages.LabTechnician", L("LabTechnician"));

        labTech.CreateChildPermission("Pages.LabTechnician.Inventory.View", L("ViewInventory"));
        labTech.CreateChildPermission("Pages.LabTechnician.Inventory.Update", L("UpdateInventory"));

        labTech.CreateChildPermission("Pages.LabTechnician.Reports.View", L("ViewLabReports"));
        labTech.CreateChildPermission("Pages.LabTechnician.TestRequests.Manage", L("TestRequests"));
        #endregion

        #region Pharmacist Permissions
        var pharmacist = pages.CreateChildPermission("Pages.Pharmacist", L("Pharmacist"));

        pharmacist.CreateChildPermission("Pages.Pharmacist.Inventory.View", L("ViewPharmaInventory"));
        pharmacist.CreateChildPermission("Pages.Pharmacist.Inventory.Update", L("UpdatePharmaInventory"));

        pharmacist.CreateChildPermission("Pages.Pharmacist.Prescriptions.View", L("ViewPrescriptions"));
        pharmacist.CreateChildPermission("Pages.Pharmacist.Prescriptions.Dispense", L("DispensePrescriptions"));
        #endregion
    }

    private static ILocalizableString L(string name)
    {
        return new LocalizableString(name, EMRSystemConsts.LocalizationSourceName);
    }
}
