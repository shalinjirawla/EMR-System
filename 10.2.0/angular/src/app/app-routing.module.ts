import { NgModule } from '@angular/core';
import { RouterModule } from '@angular/router';
import { AppRouteGuard } from '@shared/auth/auth-route-guard';
import { AppComponent } from './app.component';

@NgModule({
    imports: [
        RouterModule.forChild([
            {
                path: '',
                component: AppComponent,
                children: [
                    {
                        path: 'home',
                        loadChildren: () => import('./home/home.module').then((m) => m.HomeModule),
                        canActivate: [AppRouteGuard],
                    },
                    {
                        path: 'about',
                        loadChildren: () => import('./about/about.module').then((m) => m.AboutModule),
                        canActivate: [AppRouteGuard],
                    },
                    {
                        path: 'users',
                        loadChildren: () => import('./users/users.module').then((m) => m.UsersModule),
                        data: { permission: 'Pages.Users' },
                        canActivate: [AppRouteGuard],
                    },
                    {
                        path: 'procedure',
                        loadChildren: () => import('./procedure/procedure.module').then((m) => m.ProcedureModule),
                        data: { permission: 'Pages.Users' },
                        canActivate: [AppRouteGuard],
                    },
                    {
                        path: 'master',
                        loadChildren: () => import('./master-table/master-table.module').then((m) => m.MasterTableModule),
                        data: { permission: 'Pages.Users' },
                        canActivate: [AppRouteGuard],
                    },
                    {
                        path: 'admission',
                        loadChildren: () => import('./admission/admission.module').then((m) => m.AdmissionModule),
                        data: { permission: 'Pages.Users' },
                        canActivate: [AppRouteGuard],
                    },
                    {
                        path: 'deposit',
                        loadChildren: () => import('./deposit/deposit.module').then((m) => m.DepositModule),
                        data: { permission: 'Pages.BillingStaff' },
                        canActivate: [AppRouteGuard],
                    },
                    {
                        path: 'room',
                        loadChildren: () => import('./room/room.module').then((m) => m.RoomModule),
                        data: { permission: 'Pages.Users' },
                        canActivate: [AppRouteGuard],
                    },
                    {
                        path: 'doctors',
                        loadChildren: () => import('./doctors/doctors.module').then((m) => m.DoctorsModule),
                        data: { permission: 'Pages.Doctors' },
                        canActivate: [AppRouteGuard],
                    },
                    {
                        path: 'billing-staff',
                        loadChildren: () => import('./billing-staff/billing-staff.module').then((m) => m.BillingStaffModule),
                        data: { permission: 'Pages.BillingStaff' },
                        canActivate: [AppRouteGuard],
                    },
                    {
                        path: 'birth-death-record',
                        loadChildren: () => import('./birth-death-record/birth-death-record.module').then((m) => m.BirthDeathRecordModule),
                        data: { permission: 'Pages.Users' },
                        canActivate: [AppRouteGuard],
                    },
                    {
                        path: 'lab-technician',
                        loadChildren: () => import('./lab-technician/lab-technician.module').then((m) => m.LabTechnicianModule),
                        data: { permission: 'Pages.LabTechnician' },
                        canActivate: [AppRouteGuard],
                    },
                    {
                        path: 'nurse',
                        loadChildren: () => import('./nurse/nurse.module').then((m) => m.NurseModule),
                        data: { permission: 'Pages.Nurse' },
                        canActivate: [AppRouteGuard],
                    },
                    {
                        path: 'emergency',
                        loadChildren: () => import('./emergency/emergency.module').then((m) => m.EmergencyModule),
                        data: { permission: 'Pages.Users' },
                        canActivate: [AppRouteGuard],
                    },
                    {
                        path: 'patient',
                        loadChildren: () => import('./patient/patient.module').then((m) => m.PatientModule),
                        data: { permission: 'Pages.Nurse' },
                        canActivate: [AppRouteGuard],
                    },
                    {
                        path: 'pharmacist',
                        loadChildren: () => import('./pharmacist/pharmacist.module').then((m) => m.PharmacistModule),
                        data: { permission: 'Pages.Pharmacist' },
                        canActivate: [AppRouteGuard],
                    },
                    {
                        path: 'patient-discharge',
                        loadChildren: () => import('./patient-discharge/patient-discharge.module').then((m) => m.PatientDischargeModule),
                        // data: { permission: 'Pages.Pharmacist' },
                        canActivate: [AppRouteGuard],
                    },
                    {
                        path: 'roles',
                        loadChildren: () => import('./roles/roles.module').then((m) => m.RolesModule),
                        data: { permission: 'Pages.Roles' },
                        canActivate: [AppRouteGuard],
                    },
                    {
                        path: 'tenants',
                        loadChildren: () => import('./tenants/tenants.module').then((m) => m.TenantsModule),
                        data: { permission: 'Pages.Tenants' },
                        canActivate: [AppRouteGuard],
                    },
                    {
                        path: 'update-password',
                        loadChildren: () => import('./users/users.module').then((m) => m.UsersModule),
                        canActivate: [AppRouteGuard],
                    },
                ],
            },
        ]),
    ],
    exports: [RouterModule],
})
export class AppRoutingModule {}
