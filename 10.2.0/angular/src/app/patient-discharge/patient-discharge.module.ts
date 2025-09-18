import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { PatientDischargeRoutingModule } from './patient-discharge-routing.module';
import { CreateComponent } from './create/create.component';
import { SharedModule } from 'primeng/api';
import { ListComponent } from './list/list.component';
@NgModule({
  declarations: [],
  imports: [
    CommonModule,SharedModule,
    PatientDischargeRoutingModule,
    CreateComponent,ListComponent,
  ]
})

export class PatientDischargeModule { }
