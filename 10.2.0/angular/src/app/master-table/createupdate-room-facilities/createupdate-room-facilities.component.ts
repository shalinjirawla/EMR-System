import { Component, Injector, OnInit, EventEmitter, Output, ChangeDetectorRef, ViewChild } from '@angular/core';
import { FormsModule, NgForm } from '@angular/forms';
import { AbpModalHeaderComponent } from '../../../shared/components/modal/abp-modal-header.component';
import { AbpModalFooterComponent } from '../../../shared/components/modal/abp-modal-footer.component';
import { AppComponentBase } from '../../../shared/app-component-base';
import { LocalizePipe } from '../../../shared/pipes/localize.pipe';
import { BsModalRef } from 'ngx-bootstrap/modal';
import { RoomFacilityMasterServiceProxy, CreateUpdateRoomFacilityMasterDto, RoomFacilityMasterDto } from '../../../shared/service-proxies/service-proxies';
import { CommonModule } from '@angular/common';
import { InputTextModule } from 'primeng/inputtext';
import { ButtonModule } from 'primeng/button';

@Component({
  selector: 'app-createupdate-room-facilities',
  templateUrl: './createupdate-room-facilities.component.html',
  styleUrls: ['./createupdate-room-facilities.component.css'],
  standalone: true,
  imports: [
    FormsModule,
    CommonModule,
    AbpModalHeaderComponent,
    AbpModalFooterComponent,
    InputTextModule,
    ButtonModule
  ],
  providers: [RoomFacilityMasterServiceProxy]
})
export class CreateupdateRoomFacilitiesComponent extends AppComponentBase implements OnInit {
  @ViewChild('facilityForm', { static: true }) facilityForm: NgForm;
  @Output() onSave = new EventEmitter<void>();
  saving = false;
  id?: number;
  facility: Partial<RoomFacilityMasterDto> = {
    facilityName: ''
  };

  get isFormValid(): boolean {
    return this.facilityForm?.form?.valid;
  }

  constructor(
    injector: Injector,
    public bsModalRef: BsModalRef,
    private cd: ChangeDetectorRef,
    private _facilityService: RoomFacilityMasterServiceProxy
  ) {
    super(injector);
  }

  ngOnInit(): void {
    if (this.id) {
      this._facilityService.get(this.id).subscribe(res => {
        this.facility = res;
        this.cd.detectChanges();
      });
    }
  }

  save(): void {
    if (!this.isFormValid) {
      this.message.warn('Please enter a facility name.');
      return;
    }
    this.saving = true;
    const input = new CreateUpdateRoomFacilityMasterDto();
    input.id = this.id ?? 0;
    input.tenantId = this.appSession.tenantId;
    input.facilityName = this.facility.facilityName;

    const request = this.id
      ? this._facilityService.update(input)
      : this._facilityService.create(input);

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
