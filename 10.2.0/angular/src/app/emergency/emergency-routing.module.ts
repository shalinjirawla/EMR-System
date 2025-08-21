import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { EmergencyComponent } from './emergency.component';
import { EmergencyCaseComponent } from './emergency-case/emergency-case.component';
import { EmergencyTriageComponent } from './emergency-triage/emergency-triage.component';

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
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class EmergencyRoutingModule {}
