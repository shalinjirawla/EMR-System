import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { SharedModule } from 'primeng/api';
import { LabTechnicianRoutingModule } from './lab-technician-routing.module';
import { LabTechnicianComponent } from './lab-technician.component';
import { InventoryComponent } from './inventory/inventory.component';
import { ReportsComponent } from './reports/reports.component';
import { TestRequestsComponent } from './test-requests/test-requests.component';


@NgModule({
    imports: [
        SharedModule,
        LabTechnicianRoutingModule,
        CommonModule,
        LabTechnicianComponent,
        InventoryComponent,
        ReportsComponent,
        TestRequestsComponent
       
      
    ],
})
export class LabTechnicianModule {}
