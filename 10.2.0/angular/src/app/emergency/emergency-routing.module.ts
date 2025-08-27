import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { EmergencyComponent } from './emergency.component';
import { EmergencyCaseComponent } from './emergency-case/emergency-case.component';
import { EmergencyTriageComponent } from './emergency-triage/emergency-triage.component';
import { EmergencyPrescriptionsComponent } from './emergency-prescriptions/emergency-prescriptions.component';
import { EmergencyLabOrdersComponent } from './emergency-lab-orders/emergency-lab-orders.component';

const routes: Routes = [
  {
    path: '',
    component: EmergencyComponent,
    pathMatch: 'full',
  },
  {
    path: 'cases',
    component: EmergencyCaseComponent,
  },
  {
    path: 'triage',
    component: EmergencyTriageComponent,
  },
  {
    path: 'prescriptions',
    component: EmergencyPrescriptionsComponent,
  },
  {
    path: 'lab-order',
    component: EmergencyLabOrdersComponent,
  },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class EmergencyRoutingModule {}
