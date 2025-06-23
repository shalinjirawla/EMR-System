import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { PatientComponent } from './patient.component';
import { AppRouteGuard } from '../../shared/auth/auth-route-guard';
import { PatientsAppointmentsComponent } from './patients-appointments/patients-appointments.component';
import { PatientsLabReportsComponent } from './patients-lab-reports/patients-lab-reports.component';
import { PatientsPaymentsComponent } from './patients-payments/patients-payments.component';
import { PatientsPrescriptionsComponent } from './patients-prescriptions/patients-prescriptions.component';
import { PatientProfileComponent } from './patient-profile/patient-profile.component';





const routes: Routes = [
    {
        path: '',
        component: PatientComponent,
        canActivate: [AppRouteGuard],
        children: [
            {
                path: 'patients-appointments',
                component: PatientsAppointmentsComponent,
            },
            {
                path: 'patients-reports',
                component: PatientsLabReportsComponent,
            },
            {
                path: 'patients-payments',
                component: PatientsPaymentsComponent,
            },
            {
                path: 'patients-prescriptions',
                component: PatientsPrescriptionsComponent,
            },
            {
                path: 'patients-profile',
                component: PatientProfileComponent,
            },
            {
                path: '',
                redirectTo: 'patients-appointments',
                pathMatch: 'full'
            }
        ],
    },
];

@NgModule({
    imports: [RouterModule.forChild(routes)],
    exports: [RouterModule],
})
export class PatientRoutingModule { }
