import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { SharedModule } from 'primeng/api';
import { PharmacistRoutingModule } from './pharmacist-routing.module';
import { PharmacistComponent } from './pharmacist.component';
import { PharmacistPrescriptionsComponent } from './pharmacist-prescriptions/pharmacist-prescriptions.component';
import { MedicineListComponent } from './medicine-list/medicine-list.component';
import { PurchaseInvoiceListComponent } from './purchase-invoice-list/purchase-invoice-list.component'
import { MedicineStockComponent } from './medicine-stock/medicine-stock.component';

@NgModule({
    imports: [
        SharedModule,
        PharmacistRoutingModule,
        CommonModule,
        PharmacistComponent,
        PharmacistPrescriptionsComponent,
        MedicineListComponent,
        PurchaseInvoiceListComponent,
        MedicineStockComponent


    ],
})
export class PharmacistModule { }
