import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { SharedModule } from 'primeng/api';
import { PharmacistRoutingModule } from './pharmacist-routing.module';
import { PharmacistComponent } from './pharmacist.component';
import { PharmacistInventoryComponent } from './pharmacist-inventory/pharmacist-inventory.component';
import { PharmacistPrescriptionsComponent } from './pharmacist-prescriptions/pharmacist-prescriptions.component';


@NgModule({
    imports: [
        SharedModule,
        PharmacistRoutingModule,
        CommonModule,
        PharmacistComponent,
        PharmacistInventoryComponent,
        PharmacistPrescriptionsComponent

      
    ],
})
export class PharmacistModule {}
