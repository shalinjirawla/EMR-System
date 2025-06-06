import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { SharedModule } from 'primeng/api';
import { NurseRoutingModule } from './nurse-routing.module';
import { NurseComponent } from './nurse.component';
import { AssignedPatientsComponent } from './assigned-patients/assigned-patients.component';
import { MedicationLogComponent } from './medication-log/medication-log.component';
import { VitalsNotesComponent } from './vitals-notes/vitals-notes.component';


@NgModule({
    imports: [
        SharedModule,
        NurseRoutingModule,
        CommonModule,
        NurseComponent,
        AssignedPatientsComponent,
        MedicationLogComponent,
        VitalsNotesComponent
       
      
    ],
})
export class NurseModule {}
