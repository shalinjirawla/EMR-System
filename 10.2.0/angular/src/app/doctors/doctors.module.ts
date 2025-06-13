import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { SharedModule } from 'primeng/api';
import { DoctorsRoutingModule } from './doctors-routing.module';
import { DoctorsComponent } from './doctors.component';
import { PatientComponent } from './patient/patient.component';
import { PrescriptionsComponent } from './prescriptions/prescriptions.component';
import { VisitsComponent } from './visits/visits.component';
import { LabOrdersComponent } from './lab-orders/lab-orders.component';


@NgModule({
    imports: [
        SharedModule,
        DoctorsRoutingModule,
        CommonModule,
        DoctorsComponent,
        PatientComponent,
        VisitsComponent,
        LabOrdersComponent,
        PrescriptionsComponent
      
    ],
})
export class DoctorsModule {}
