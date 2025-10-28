import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { BirthDeathRecordComponent } from './birth-death-record.component';
import { AppRouteGuard } from '../../shared/auth/auth-route-guard';
import { BirthRecordComponent } from './birth-record/birth-record.component';
import { DeathRecordComponent } from './death-record/death-record.component';

const routes: Routes = [
    {
        path: '',
        component: BirthDeathRecordComponent,
        canActivate: [AppRouteGuard],
        children: [
            {
                path: 'birth-record',
                component: BirthRecordComponent,
            },
            {
                path: 'death-record',
                component: DeathRecordComponent,
            },
            {
                path: '',
                redirectTo: 'birth-record',
                pathMatch: 'full'
            }
        ],
    },
];
@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class BirthDeathRecordRoutingModule {}
