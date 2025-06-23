import { Component, OnInit } from '@angular/core';
import { AbpModalFooterComponent } from '@shared/components/modal/abp-modal-footer.component';
import { AbpModalHeaderComponent } from '@shared/components/modal/abp-modal-header.component';
import { LocalizePipe } from '@shared/pipes/localize.pipe';
import { BsModalRef } from 'ngx-bootstrap/modal';
import { ButtonModule } from 'primeng/button';
import { TabsModule } from 'primeng/tabs';
import { StepperModule } from 'primeng/stepper';
import { TableModule } from 'primeng/table';
import { AvatarModule } from 'primeng/avatar';
import { ProgressBarModule } from 'primeng/progressbar';
@Component({
  selector: 'app-patient-profile',
  imports: [AbpModalHeaderComponent, AbpModalFooterComponent, TableModule, ProgressBarModule,
    LocalizePipe, ButtonModule, TabsModule, StepperModule, AvatarModule],
  templateUrl: './patient-profile.component.html',
  styleUrl: './patient-profile.component.css'
})
export class PatientProfileComponent implements OnInit {
  constructor(public bsModalRef: BsModalRef,) { }

  ngOnInit() {
  }
}
