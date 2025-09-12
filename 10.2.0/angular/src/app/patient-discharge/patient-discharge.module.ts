import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { PatientDischargeRoutingModule } from './patient-discharge-routing.module';
import { CreateComponent } from './create/create.component';
import { SharedModule } from 'primeng/api';
@NgModule({
  declarations: [],
  imports: [
    CommonModule,SharedModule,
    PatientDischargeRoutingModule,
    CreateComponent
  ]
})

export class PatientDischargeModule { }
