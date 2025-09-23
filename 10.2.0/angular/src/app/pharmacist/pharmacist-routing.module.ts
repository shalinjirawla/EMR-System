import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { PharmacistComponent } from './pharmacist.component';
import { AppRouteGuard } from '../../shared/auth/auth-route-guard';
import { PharmacistPrescriptionsComponent } from './pharmacist-prescriptions/pharmacist-prescriptions.component';
import { PharmacistInventoryComponent } from './pharmacist-inventory/pharmacist-inventory.component';
import {MedicineListComponent} from './medicine-list/medicine-list.component'
import { PurchaseInvoiceListComponent } from './purchase-invoice-list/purchase-invoice-list.component';
import {MedicineStockComponent} from './medicine-stock/medicine-stock.component';


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
                path:'medicine-list',
                component: MedicineListComponent
            },
            {
                path:'purchase-medicine',
                component:PurchaseInvoiceListComponent
            },
            {
                path:'medicine-stock',
                component:MedicineStockComponent
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
