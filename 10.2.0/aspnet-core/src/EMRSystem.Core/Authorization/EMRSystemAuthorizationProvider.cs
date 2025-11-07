using Abp.Authorization;
using Abp.Localization;
using Abp.MultiTenancy;

namespace EMRSystem.Authorization;

public class EMRSystemAuthorizationProvider : AuthorizationProvider
{
        public override void SetPermissions(IPermissionDefinitionContext context)
        {
            var pages = context.CreatePermission("Pages", L("Pages"));

            #region 🔹 Admin Section
            context.CreatePermission("Pages.Users", L("UserManagement"));
            context.CreatePermission("Pages.Roles", L("RoleManagement"));
            context.CreatePermission(PermissionNames.Pages_Users_Activation, L("UsersActivation"));
            context.CreatePermission("Pages.Tenants", L("TenantManagement"), multiTenancySides: MultiTenancySides.Host);
            context.CreatePermission("Pages.Admission", L("PatientAdmission"));
            context.CreatePermission("Pages.PatientDischarge", L("DischargedPatient"));
            #endregion

            #region 🔹 Emergency Portal
            var emergency = pages.CreateChildPermission("Pages.Emergency", L("EmergencyPortal"));
            emergency.CreateChildPermission("Pages.Emergency.Cases", L("EmergencyCases"));
            emergency.CreateChildPermission("Pages.Emergency.Triage", L("Triage"));
            emergency.CreateChildPermission("Pages.Emergency.Prescriptions", L("EmergencyPrescriptions"));
            emergency.CreateChildPermission("Pages.Emergency.LabOrder", L("EmergencyLabOrder"));
            #endregion

            #region 🔹 Nurse Portal
            var nurse = pages.CreateChildPermission("Pages.Nurse", L("NursePortal"));
            nurse.CreateChildPermission("Pages.Nurse.Appointments.Manage", L("ManageAppointments"));
            nurse.CreateChildPermission("Pages.Nurse.Patients.View", L("PatientAssignments"));
            nurse.CreateChildPermission("Pages.Nurse.Vitals.Manage", L("VitalsMonitoring"));
            nurse.CreateChildPermission("Pages.Nurse.Medication.Manage", L("MedicationOrders"));
            #endregion

            #region 🔹 Doctor Portal
            var doctor = pages.CreateChildPermission("Pages.Doctor", L("DoctorPortal"));
            doctor.CreateChildPermission("Pages.Doctor.Appointments.View", L("Appointments"));
            doctor.CreateChildPermission("Pages.Doctor.Patients.View", L("AssignedPatients"));
            doctor.CreateChildPermission("Pages.Doctor.Prescriptions.Manage", L("Prescriptions"));
            doctor.CreateChildPermission("Pages.Doctor.LabOrders.Manage", L("LabOrders"));
            doctor.CreateChildPermission("Pages.Doctor.ConsultationRequests.Manage", L("ConsultationRequests"));
            #endregion

            #region 🔹 Procedure Portal
            var procedure = pages.CreateChildPermission("Pages.Procedure", L("ProcedurePortal"));
            procedure.CreateChildPermission("Pages.Procedure.Requests.View", L("ProcedureRequests"));
            procedure.CreateChildPermission("Pages.Procedure.Receipts.View", L("ProcedureReceipts"));
            #endregion

            #region 🔹 Lab Technician Portal
            var lab = pages.CreateChildPermission("Pages.Lab", L("LabTechnicianPortal"));
            lab.CreateChildPermission("Pages.Lab.TestRequests.Manage", L("TestRequests"));
            lab.CreateChildPermission("Pages.Lab.Receipts.View", L("LabReceipts"));
            #endregion

            #region 🔹 Pharmacist Portal
            var pharma = pages.CreateChildPermission("Pages.Pharmacy", L("PharmacyPortal"));
            pharma.CreateChildPermission("Pages.Pharmacy.MedicineList.View", L("MedicineList"));
            pharma.CreateChildPermission("Pages.Pharmacy.Purchase.Manage", L("PurchaseMedicine"));
            pharma.CreateChildPermission("Pages.Pharmacy.Stock.View", L("MedicineStock"));
            pharma.CreateChildPermission("Pages.Pharmacy.Prescriptions.View", L("PrescriptionFulfillment"));
            #endregion

            #region 🔹 Billing Staff Portal
            var billing = pages.CreateChildPermission("Pages.Billing", L("BillingPortal"));
            billing.CreateChildPermission("Pages.Billing.InsuranceClaims.View", L("InsuranceClaim"));
            billing.CreateChildPermission("Pages.Billing.Invoices.View", L("Invoices"));
            billing.CreateChildPermission("Pages.Billing.Deposits.Manage", L("PatientDeposits"));
            #endregion

            #region 🔹 Birth/Death Portal
            var birthDeath = pages.CreateChildPermission("Pages.BirthDeath", L("BirthDeathPortal"));
            birthDeath.CreateChildPermission("Pages.BirthDeath.BirthRecord.View", L("BirthRecord"));
            birthDeath.CreateChildPermission("Pages.BirthDeath.DeathRecord.View", L("DeathRecord"));
            #endregion

            #region 🔹 Masters
            var master = pages.CreateChildPermission("Pages.Master", L("Masters"));

            // Laboratory Settings
            var labMaster = master.CreateChildPermission("Pages.Master.LabSettings", L("LaboratorySettings"));
            labMaster.CreateChildPermission("Pages.Master.MeasureUnits.View", L("MeasurementUnits"));
            labMaster.CreateChildPermission("Pages.Master.LabTests.View", L("LabTests"));
            labMaster.CreateChildPermission("Pages.Master.TestResultLimits.View", L("TestResultLimits"));
            labMaster.CreateChildPermission("Pages.Master.LabReportTypes.View", L("LabReportTypes"));
            labMaster.CreateChildPermission("Pages.Master.LabTestItems.View", L("LabTestItems"));
            labMaster.CreateChildPermission("Pages.Master.HealthPackages.View", L("HealthPackages"));

            // Room Management
            var room = master.CreateChildPermission("Pages.Master.RoomManagement", L("RoomManagement"));
            room.CreateChildPermission("Pages.Master.RoomFacilities.View", L("RoomFacilities"));
            room.CreateChildPermission("Pages.Master.RoomTypes.View", L("RoomTypes"));
            room.CreateChildPermission("Pages.Master.Rooms.View", L("Rooms"));
            room.CreateChildPermission("Pages.Master.Beds.View", L("Beds"));

            // Medicine Module
            var medicine = master.CreateChildPermission("Pages.Master.MedicineModule", L("MedicineModule"));
            medicine.CreateChildPermission("Pages.Master.MedicineFormType.View", L("MedicineFormType"));
            medicine.CreateChildPermission("Pages.Master.MedicineStrengthType.View", L("MedicineStrengthType"));

            // Other Masters
            master.CreateChildPermission("Pages.Master.InsuranceMaster.View", L("InsuranceMaster"));
            master.CreateChildPermission("Pages.Master.ProcedureTypes.View", L("ProcedureTypes"));
            master.CreateChildPermission("Pages.Master.Departments.View", L("DepartmentsTypes"));
            master.CreateChildPermission("Pages.Master.DoctorMaster.View", L("DoctorMasterData"));
            master.CreateChildPermission("Pages.Master.AppointmentTypes.View", L("AppointmentTypes"));
            master.CreateChildPermission("Pages.Master.EmergencyCharges.View", L("EmergencyCharges"));
            #endregion
        }
        

        private static ILocalizableString L(string name)
    {
        return new LocalizableString(name, EMRSystemConsts.LocalizationSourceName);
    }
}

//public override void SetPermissions(IPermissionDefinitionContext context)
//{
//    var pages = context.CreatePermission("Pages", L("Pages"));

//    #region Admin
//    context.CreatePermission(PermissionNames.Pages_Users, L("Users"));
//    context.CreatePermission(PermissionNames.Pages_Users_Activation, L("UsersActivation"));
//    context.CreatePermission(PermissionNames.Pages_Roles, L("Roles"));
//    context.CreatePermission(PermissionNames.Pages_Tenants, L("Tenants"), multiTenancySides: MultiTenancySides.Host);

//    #endregion

//    #region Doctor Permissions
//    var doctors = pages.CreateChildPermission("Pages.Doctors", L("Doctors"));

//    var appointments = doctors.CreateChildPermission("Pages.Doctors.Appointments", L("Appointments"));
//    appointments.CreateChildPermission("Pages.Doctors.Appointments.View", L("ViewAppointments"));
//    appointments.CreateChildPermission("Pages.Doctors.Appointments.StatusUpdate", L("UpdateAppointmentStatus"));
//    appointments.CreateChildPermission("Pages.Doctors.Appointments.Schedule", L("ScheduleAppointments"));

//    var patients = doctors.CreateChildPermission("Pages.Doctors.Patients", L("Patients"));
//    patients.CreateChildPermission("Pages.Doctors.Patients.View", L("ViewPatients"));
//    patients.CreateChildPermission("Pages.Doctors.Patients.History", L("ViewPatientHistory"));

//    var prescriptions = doctors.CreateChildPermission("Pages.Doctors.Prescriptions", L("Prescriptions"));
//    prescriptions.CreateChildPermission("Pages.Doctors.Prescriptions.Create", L("CreatePrescriptions"));
//    prescriptions.CreateChildPermission("Pages.Doctors.Prescriptions.Update", L("UpdatePrescriptions"));

//    var visits = doctors.CreateChildPermission("Pages.Doctors.Visits", L("Visits"));
//    visits.CreateChildPermission("Pages.Doctors.Visits.View", L("ViewVisits"));
//    visits.CreateChildPermission("Pages.Doctors.Visits.Complete", L("CompleteVisit"));

//    var labOrders = doctors.CreateChildPermission("Pages.Doctors.LabOrders", L("LabOrders"));
//    labOrders.CreateChildPermission("Pages.Doctors.LabOrders.Create", L("CreateLabOrders"));
//    labOrders.CreateChildPermission("Pages.Doctors.LabOrders.View", L("ViewLabOrders"));
//    #endregion

//    #region Nurse Permissions
//    var nurse = pages.CreateChildPermission("Pages.Nurse", L("Nurse"));

//    var assignedPatients = nurse.CreateChildPermission("Pages.Nurse.Patients", L("AssignedPatients"));
//    assignedPatients.CreateChildPermission("Pages.Nurse.Patients.View", L("ViewAssignedPatients"));

//    var medication = nurse.CreateChildPermission("Pages.Nurse.Medication", L("MedicationLog"));
//    medication.CreateChildPermission("Pages.Nurse.Medication.View", L("ViewMedicationLog"));
//    medication.CreateChildPermission("Pages.Nurse.Medication.Update", L("UpdateMedicationLog"));

//    var vitals = nurse.CreateChildPermission("Pages.Nurse.Vitals", L("VitalsNotes"));
//    vitals.CreateChildPermission("Pages.Nurse.Vitals.View", L("ViewVitals"));
//    vitals.CreateChildPermission("Pages.Nurse.Vitals.Create", L("CreateVitals"));
//    vitals.CreateChildPermission("Pages.Nurse.Vitals.Update", L("UpdateVitals"));
//    #endregion

//    #region Billing Staff Permissions
//    var billing = pages.CreateChildPermission("Pages.BillingStaff", L("BillingStaff"));

//    billing.CreateChildPermission("Pages.BillingStaff.Invoices.Create", L("CreateInvoices"));
//    billing.CreateChildPermission("Pages.BillingStaff.Invoices.View", L("ViewInvoices"));
//    billing.CreateChildPermission("Pages.BillingStaff.Invoices.Update", L("UpdateInvoices"));

//    billing.CreateChildPermission("Pages.BillingStaff.Payments.Create", L("CreatePayments"));
//    billing.CreateChildPermission("Pages.BillingStaff.Payments.View", L("ViewPayments"));

//    billing.CreateChildPermission("Pages.BillingStaff.Insurance.View", L("ViewInsurance"));
//    billing.CreateChildPermission("Pages.BillingStaff.Reports.View", L("ViewReports"));
//    #endregion

//    #region Lab Technician Permissions
//    var labTech = pages.CreateChildPermission("Pages.LabTechnician", L("LabTechnician"));

//    labTech.CreateChildPermission("Pages.LabTechnician.Inventory.View", L("ViewInventory"));
//    labTech.CreateChildPermission("Pages.LabTechnician.Inventory.Update", L("UpdateInventory"));

//    labTech.CreateChildPermission("Pages.LabTechnician.Reports.View", L("ViewLabReports"));
//    labTech.CreateChildPermission("Pages.LabTechnician.TestRequests.Manage", L("TestRequests"));
//    #endregion

//    #region Pharmacist Permissions
//    var pharmacist = pages.CreateChildPermission("Pages.Pharmacist", L("Pharmacist"));

//    pharmacist.CreateChildPermission("Pages.Pharmacist.Inventory.View", L("ViewPharmaInventory"));
//    pharmacist.CreateChildPermission("Pages.Pharmacist.Inventory.Update", L("UpdatePharmaInventory"));

//    pharmacist.CreateChildPermission("Pages.Pharmacist.Prescriptions.View", L("ViewPrescriptions"));
//    pharmacist.CreateChildPermission("Pages.Pharmacist.Prescriptions.Dispense", L("DispensePrescriptions"));
//    #endregion
//}