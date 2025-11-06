import { Component, Injector, OnInit, EventEmitter, Output, ViewChild } from '@angular/core';
import { FormsModule, NgForm } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { AbpModalHeaderComponent } from '@shared/components/modal/abp-modal-header.component';
import { AbpModalFooterComponent } from '@shared/components/modal/abp-modal-footer.component';
import { InputTextModule } from 'primeng/inputtext';
import { ButtonModule } from 'primeng/button';
import { BsModalRef } from 'ngx-bootstrap/modal';
import { ChangeDetectorRef } from '@angular/core';
import { AppComponentBase } from '@shared/app-component-base';
import { InsuranceMasterServiceProxy, CreateUpdateInsuranceMasterDto, InsuranceMasterDto } from '@shared/service-proxies/service-proxies';
import { InputSwitchModule } from 'primeng/inputswitch';

@Component({
  selector: 'app-createupdate-insurance-master',
  standalone: true,
  imports: [AbpModalHeaderComponent, AbpModalFooterComponent, FormsModule, CommonModule,
    InputTextModule,ButtonModule, InputSwitchModule],
  providers: [InsuranceMasterServiceProxy],
  templateUrl: './createupdate-insurance-master.component.html',
  styleUrl: './createupdate-insurance-master.component.css'
})

export class CreateupdateInsuranceMasterComponent extends AppComponentBase implements OnInit {
  @ViewChild('insuranceForm', { static: true }) insuranceForm: NgForm;
  @Output() onSave = new EventEmitter<void>();

  saving = false;
  id?: number;

  // DTO instance
  insurance: CreateUpdateInsuranceMasterDto = new CreateUpdateInsuranceMasterDto();


  constructor(
    injector: Injector,
    public bsModalRef: BsModalRef,
    private _insuranceService: InsuranceMasterServiceProxy,
    private cd: ChangeDetectorRef
  ) {
    super(injector);
  }

  get isFormValid(): boolean {
    return this.insuranceForm?.form?.valid;
  }

  ngOnInit(): void {
    // Default values
    this.insurance.tenantId = abp.session.tenantId;
    this.insurance.insuranceName = '';
    this.insurance.isActive = true;

    if (this.id) {
      this._insuranceService.get(this.id).subscribe((res: InsuranceMasterDto) => {
        this.insurance = Object.assign(new CreateUpdateInsuranceMasterDto(), res);
        this.cd.detectChanges();
      });
    }
  }

  save(): void {
    if (!this.isFormValid) {
      this.message.warn('Please fill required fields.');
      return;
    }

    this.saving = true;

    const input = Object.assign(new CreateUpdateInsuranceMasterDto(), this.insurance);
    input.tenantId = this.appSession.tenantId;

    debugger
    const request = this.id ? this._insuranceService.update(input) : this._insuranceService.create(input);

    request.subscribe({
      next: () => {
        this.notify.info(this.l('SavedSuccessfully'));
        this.onSave.emit();
        this.bsModalRef.hide();
      },
      error: () => { this.saving = false; }
    });
  }
}
