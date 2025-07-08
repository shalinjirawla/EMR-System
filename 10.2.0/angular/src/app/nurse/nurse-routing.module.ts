import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { NurseComponent } from './nurse.component';
import { AppRouteGuard } from '@shared/auth/auth-route-guard';
import { AssignedPatientsComponent } from './assigned-patients/assigned-patients.component';
import { OrderMedicineComponent } from './order-medicine/order-medicine.component';
import { VitalsNotesComponent } from './vitals-notes/vitals-notes.component';
import { AppointmentsComponent } from './appointments/appointments.component'
const routes: Routes = [
    {
        path: '',
        component: NurseComponent,
        canActivate: [AppRouteGuard],
        children: [
            {
                path: 'appointments',
                component: AppointmentsComponent,
            },
            {
                path: 'assigned-patients',
                component: AssignedPatientsComponent,
            },
            {
                path: 'order-medicine',
                component: OrderMedicineComponent,
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
export class NurseRoutingModule { }
