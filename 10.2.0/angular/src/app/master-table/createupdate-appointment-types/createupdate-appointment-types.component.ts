import { Component, Injector, OnInit, EventEmitter, Output, ChangeDetectorRef, ViewChild } from '@angular/core';
import { FormsModule, NgForm } from '@angular/forms';
import { AbpModalHeaderComponent } from '../../../shared/components/modal/abp-modal-header.component';
import { AbpModalFooterComponent } from '../../../shared/components/modal/abp-modal-footer.component';
import { AppComponentBase } from '../../../shared/app-component-base';
import { LocalizePipe } from '../../../shared/pipes/localize.pipe';
import { BsModalRef } from 'ngx-bootstrap/modal';
import { RoomFacilityMasterServiceProxy, CreateUpdateRoomFacilityMasterDto, RoomFacilityMasterDto, AppointmentTypeServiceProxy, AppointmentTypeDto, CreateUpdateAppointmentTypeDto } from '../../../shared/service-proxies/service-proxies';
import { CommonModule } from '@angular/common';
import { InputTextModule } from 'primeng/inputtext';
import { ButtonModule } from 'primeng/button';



@Component({
  selector: 'app-createupdate-appointment-types',
  imports: [FormsModule, CommonModule, AbpModalHeaderComponent, AbpModalFooterComponent, InputTextModule, ButtonModule],
  providers: [AppointmentTypeServiceProxy],
  standalone: true,
  templateUrl: './createupdate-appointment-types.component.html',
  styleUrl: './createupdate-appointment-types.component.css'
})
export class CreateupdateAppointmentTypesComponent extends AppComponentBase implements OnInit {
  @ViewChild('appointmentTypeForm', { static: true }) appointmentTypeForm: NgForm;
  @Output() onSave = new EventEmitter<void>();
  saving = false;
  id?: number;
  appointmentType: Partial<AppointmentTypeDto> = {
    name: '',
    fee: 0,
    description: ''
  };

  get isFormValid(): boolean {
    return this.appointmentTypeForm?.form?.valid;
  }

  constructor(
    injector: Injector,
    public bsModalRef: BsModalRef,
    private cd: ChangeDetectorRef,
    private _appointmentTypeService: AppointmentTypeServiceProxy
  ) {
    super(injector);
  }

  ngOnInit(): void {
    if (this.id) {
      this._appointmentTypeService.get(this.id).subscribe(res => {
        this.appointmentType = res;
        this.cd.detectChanges();
      });
    }
  }

  save(): void {
    if (!this.isFormValid) {
      this.message.warn('Please enter a appointment type name.');
      return;
    }
    this.saving = true;
    const input = new CreateUpdateAppointmentTypeDto();
    input.id = this.id ?? 0;
    input.tenantId = this.appSession.tenantId;
    input.name = this.appointmentType.name;
    input.fee = this.appointmentType.fee;
    input.description = this.appointmentType.description;

    const request = this.id
      ? this._appointmentTypeService.update(input)
      : this._appointmentTypeService.create(input);

    request.subscribe({
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