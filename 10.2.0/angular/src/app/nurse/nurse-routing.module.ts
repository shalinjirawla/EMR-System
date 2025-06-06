import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { NurseComponent } from './nurse.component';
import { AppRouteGuard } from '@shared/auth/auth-route-guard';
import { AssignedPatientsComponent } from './assigned-patients/assigned-patients.component';
import { MedicationLogComponent } from './medication-log/medication-log.component';
import { VitalsNotesComponent } from './vitals-notes/vitals-notes.component';





const routes: Routes = [
    {
        path: '',
        component: NurseComponent,
          canActivate: [AppRouteGuard],
        children: [
            {
                path: 'assigned-patients',
                component: AssignedPatientsComponent,
            },
            {
                path: 'medication-log',
                component: MedicationLogComponent,
            },
            {
                path: 'vitals-notes',
                component: VitalsNotesComponent,
            },
            {
                path: '',
                redirectTo: 'assigned-patients',
                pathMatch: 'full'
            }
        ],
    },
];

@NgModule({
    imports: [RouterModule.forChild(routes)],
    exports: [RouterModule],
})
export class NurseRoutingModule {}
