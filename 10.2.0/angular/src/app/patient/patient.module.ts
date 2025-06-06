import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { SharedModule } from 'primeng/api';
import { PatientRoutingModule } from './patient-routing.module';
import { PatientComponent } from './patient.component';
import { PatientsAppointmentsComponent } from './patients-appointments/patients-appointments.component';
import { PatientsLabReportsComponent } from './patients-lab-reports/patients-lab-reports.component';
import { PatientsPaymentsComponent } from './patients-payments/patients-payments.component';
import { PatientsPrescriptionsComponent } from './patients-prescriptions/patients-prescriptions.component';


@NgModule({
    imports: [
        SharedModule,
        PatientRoutingModule,
        CommonModule,
        PatientComponent,
        PatientsAppointmentsComponent,
        PatientsLabReportsComponent,
        PatientsPaymentsComponent,
        PatientsPrescriptionsComponent
       
      
    ],
})
export class PatientModule {}
