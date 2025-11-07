import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { SharedModule } from 'primeng/api';
import { PatientRoutingModule } from './patient-routing.module';
import { PatientComponent } from './patient.component';


@NgModule({
    imports: [
        SharedModule,
        PatientRoutingModule,
        CommonModule,
        PatientComponent
       
      
    ],
})
export class PatientModule {}
