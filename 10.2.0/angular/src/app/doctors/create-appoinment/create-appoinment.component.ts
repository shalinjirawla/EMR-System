import { Component, Injector, OnInit, EventEmitter, Output, ChangeDetectorRef, ViewChild } from '@angular/core';
import { forEach as _forEach, map as _map } from 'lodash-es';
import { FormsModule, NgForm } from '@angular/forms';
import { AbpModalHeaderComponent } from '../../../shared/components/modal/abp-modal-header.component';
import { AbpValidationSummaryComponent } from '../../../shared/components/validation/abp-validation.summary.component';
import { EqualValidator } from '../../../shared/directives/equal-validator.directive';
import { AbpModalFooterComponent } from '../../../shared/components/modal/abp-modal-footer.component';
import { LocalizePipe } from '@shared/pipes/localize.pipe';
import { CommonModule } from '@node_modules/@angular/common';
import { AppComponentBase } from '@shared/app-component-base';
import { BsModalRef } from '@node_modules/ngx-bootstrap/modal';


@Component({
  selector: 'app-create-appoinment',
  imports: [
    FormsModule,
    AbpModalHeaderComponent,
    AbpModalFooterComponent,
    CommonModule
  ],
  standalone: true,
  templateUrl: './create-appoinment.component.html',
  styleUrl: './create-appoinment.component.css'
})
export class CreateAppoinmentComponent extends AppComponentBase implements OnInit {
  @ViewChild('createAppoinmentModal', { static: true }) createAppoinmentModal: NgForm;
  saving = false;

  get isFormValid(): boolean {
    const mainFormValid = this.createAppoinmentModal?.form?.valid;
    return mainFormValid;
  }
  constructor(
    injector: Injector,
    public bsModalRef: BsModalRef,
    private cd: ChangeDetectorRef,
  ) {
    super(injector);
  }

  ngOnInit(): void {
    throw new Error('Method not implemented.');
  }

  save(): void { }

}
