import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { PharmacistComponent } from './pharmacist.component';
import { AppRouteGuard } from '../../shared/auth/auth-route-guard';
import { PharmacistPrescriptionsComponent } from './pharmacist-prescriptions/pharmacist-prescriptions.component';
import { PharmacistInventoryComponent } from './pharmacist-inventory/pharmacist-inventory.component';



const routes: Routes = [
    {
        path: '',
        component: PharmacistComponent,
          canActivate: [AppRouteGuard],
        children: [
            {
                path: 'pharmacist-prescriptions',
                component: PharmacistPrescriptionsComponent,
            },
            {
                path: 'pharmacist-inventory',
                component: PharmacistInventoryComponent,
            },
            {
                path: '',
                redirectTo: 'pharmacist-prescriptions',
                pathMatch: 'full'
            }
        ],
    },
];

@NgModule({
    imports: [RouterModule.forChild(routes)],
    exports: [RouterModule],
})
export class PharmacistRoutingModule {}
