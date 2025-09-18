import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { CreateComponent } from './create/create.component';
import { ListComponent } from './list/list.component';
import { AppRouteGuard } from '../../shared/auth/auth-route-guard';
import { PatientDischargeComponent } from './patient-discharge.component';
const routes: Routes = [
  {
    path: '',
    component: PatientDischargeComponent,
    canActivate: [AppRouteGuard],
    children: [
      {
        path: 'create/:id',
        component: CreateComponent,
      },
      {
        path: 'list',
        component: ListComponent,
      },
      {
        path: '',
        redirectTo: 'patient-discharge',
        pathMatch: 'full'
      }
    ],
  },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class PatientDischargeRoutingModule { }
