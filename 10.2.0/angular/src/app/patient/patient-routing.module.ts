import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { PatientComponent } from './patient.component';
import { AppRouteGuard } from '../../shared/auth/auth-route-guard';
import { PatientProfileComponent } from './patient-profile/patient-profile.component';





const routes: Routes = [
    {
        path: '',
        component: PatientComponent,
        canActivate: [AppRouteGuard],
        children: [
            
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
