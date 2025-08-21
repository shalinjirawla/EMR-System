import { NgModule } from '@angular/core';
import { SharedModule } from 'primeng/api';
import { EmergencyComponent } from './emergency.component';
import { EmergencyRoutingModule } from './emergency-routing.module';
import {EmergencyCaseComponent} from './emergency-case/emergency-case.component'
import {EmergencyTriageComponent} from './emergency-triage/emergency-triage.component'

@NgModule({
    imports: [SharedModule, EmergencyComponent, EmergencyRoutingModule,EmergencyCaseComponent,EmergencyTriageComponent],
})
export class EmergencyModule {}
