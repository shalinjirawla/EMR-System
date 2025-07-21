import { NgModule } from '@angular/core';
import { AdmissionRoutingModule } from './admission-routing.module';
import { AdmissionComponent } from './admission.component';
import { SharedModule } from 'primeng/api';

@NgModule({
    imports: [SharedModule, AdmissionComponent, AdmissionRoutingModule],
})
export class AdmissionModule {}
