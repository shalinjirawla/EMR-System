import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { DoctorsComponent } from './doctors.component';
import { PatientComponent } from './patient/patient.component';
import { PrescriptionsComponent } from './prescriptions/prescriptions.component';
import { VisitsComponent } from './visits/visits.component';
import { AppRouteGuard } from '@shared/auth/auth-route-guard';
import { LabOrdersComponent } from './lab-orders/lab-orders.component';


const routes: Routes = [
    {
        path: '',
        component: DoctorsComponent,
          canActivate: [AppRouteGuard],
        children: [
            {
                path: 'patients',
                component: PatientComponent,
            },
            {
                path: 'prescriptions',
                component: PrescriptionsComponent,
            },
            {
                path: 'lab-order',
                component: LabOrdersComponent,
            },
            {
                path: 'visits',
                component: VisitsComponent,
            },
            {
                path: '',
                redirectTo: 'patients',
                pathMatch: 'full'
            }
        ],
    },
];

@NgModule({
    imports: [RouterModule.forChild(routes)],
    exports: [RouterModule],
})
export class DoctorsRoutingModule {}
