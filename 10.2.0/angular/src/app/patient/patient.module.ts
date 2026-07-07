import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { SharedModule } from 'primeng/api';
import { PatientRoutingModule } from './patient-routing.module';
import { PatientComponent } from './patient.component';


import { AbhaCreationComponent } from './patient-profile/abha-creation/abha-creation.component';
import { FormsModule } from '@angular/forms';
import { AbhaLoginComponent } from './patient-profile/abha-login/abha-login.component';

import { AbhaDashboardComponent } from './patient-profile/abha-dashboard/abha-dashboard.component';

@NgModule({
    imports: [
        SharedModule,
        PatientRoutingModule,
        CommonModule,
        PatientComponent,
        FormsModule,
        AbhaCreationComponent,
        AbhaLoginComponent,
        AbhaDashboardComponent
    ],
    declarations: [

    ]
})
export class PatientModule { }
