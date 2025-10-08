import {
  Component, Injector, OnInit, EventEmitter, Output,
  ChangeDetectorRef, ViewChild
} from '@angular/core';
import { AbpModalHeaderComponent } from '../../../shared/components/modal/abp-modal-header.component';
import { AbpModalFooterComponent } from '../../../shared/components/modal/abp-modal-footer.component';
import { NgForm, FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { InputTextModule } from 'primeng/inputtext';
import { ButtonModule } from 'primeng/button';
import { BsModalRef } from 'ngx-bootstrap/modal';
import { CreateUpdateEmergencyMasterDto, EmergencyMasterServiceProxy } from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/app-component-base';
@Component({
  selector: 'app-createupdate-emergency-charges',
  imports: [FormsModule, CommonModule, AbpModalHeaderComponent, AbpModalFooterComponent,
    InputTextModule, ButtonModule
  ],
  templateUrl: './createupdate-emergency-charges.component.html',
  styleUrl: './createupdate-emergency-charges.component.css',
  providers: [EmergencyMasterServiceProxy]
})
export class CreateupdateEmergencyChargesComponent extends AppComponentBase implements OnInit {
  @ViewChild('emergencyMasterForm', { static: true }) emergencyMasterForm: NgForm;
  @Output() onSave = new EventEmitter<void>();

  saving = false;
  id?: number;
  fee: number;
  title: string;
  constructor(
    injector: Injector,
    public bsModalRef: BsModalRef,
    private cd: ChangeDetectorRef,
    private _emergencyMasterService: EmergencyMasterServiceProxy
  ) {
    super(injector);
  }
  ngOnInit(): void {
    if (this.id) {
      this._emergencyMasterService.get(this.id).subscribe(res => {
        this.title = res.title
        this.fee = res.fee;
        this.cd.detectChanges();
      });
    }
  }
  get isFormValid(): boolean {
    return this.emergencyMasterForm?.form?.valid;
  }
  save(): void {
    if (!this.isFormValid) {
      return;
    }
    this.saving = true;
    const input = new CreateUpdateEmergencyMasterDto();
    input.tenantId = abp.session.tenantId;
    input.title = this.title;
    input.fee = this.fee;

    if (this.id) {
      input.id = this.id;
      this.Edit(input);
    } else {
      this.Create(input);
    }
  }

  Create(input: CreateUpdateEmergencyMasterDto) {
    this._emergencyMasterService.create(input).subscribe({
      next: (res) => {
        this.notify.info(this.l('SavedSuccessfully'));
        this.bsModalRef.hide();
        this.onSave.emit();
      }, error: (err) => {
        this.saving = false;
      }
    })
  }
  Edit(input: CreateUpdateEmergencyMasterDto) {
    this._emergencyMasterService.update(input).subscribe({
      next: () => {
        this.notify.info(this.l('SavedSuccessfully'));
        this.bsModalRef.hide();
        this.onSave.emit();
      },
      error: () => {
        this.saving = false;
      }
    });
  }
}
