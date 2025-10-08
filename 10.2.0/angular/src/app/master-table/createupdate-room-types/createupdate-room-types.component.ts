import { Component, Injector, OnInit, EventEmitter, Output, ChangeDetectorRef, ViewChild } from '@angular/core';
import { FormsModule, NgForm } from '@angular/forms';
import { AbpModalHeaderComponent } from '../../../shared/components/modal/abp-modal-header.component';
import { AbpModalFooterComponent } from '../../../shared/components/modal/abp-modal-footer.component';
import { AppComponentBase } from '../../../shared/app-component-base';
import { LocalizePipe } from '../../../shared/pipes/localize.pipe';
import { BsModalRef } from 'ngx-bootstrap/modal';
import { RoomFacilityMasterServiceProxy, CreateUpdateRoomFacilityMasterDto, RoomFacilityMasterDto, RoomTypeMasterServiceProxy, RoomTypeMasterDto, CreateUpdateRoomTypeMasterDto } from '../../../shared/service-proxies/service-proxies';
import { CommonModule } from '@angular/common';
import { InputTextModule } from 'primeng/inputtext';
import { ButtonModule } from 'primeng/button';
import { MultiSelectModule } from 'primeng/multiselect';

@Component({
  selector: 'app-createupdate-room-types',
  imports: [AbpModalHeaderComponent, AbpModalFooterComponent, FormsModule, CommonModule,
     InputTextModule, ButtonModule,MultiSelectModule ],
  providers: [RoomTypeMasterServiceProxy,RoomFacilityMasterServiceProxy],
  templateUrl: './createupdate-room-types.component.html',
  styleUrl: './createupdate-room-types.component.css'
})
export class CreateupdateRoomTypesComponent extends AppComponentBase implements OnInit {
  @ViewChild('roomTypeForm', { static: true }) roomTypeForm: NgForm;
  @Output() onSave = new EventEmitter<void>();
  saving = false;
  id?: number;
  roomType: Partial<RoomTypeMasterDto> = {
    typeName: ''
  };
  facilityOptions: { label: string; value: number }[] = [];


  get isFormValid(): boolean {
    return this.roomTypeForm?.form?.valid;
  }

  constructor(
    injector: Injector,
    public bsModalRef: BsModalRef,
    private cd: ChangeDetectorRef,
    private _roomTypeService: RoomTypeMasterServiceProxy,
    private _roomFacilityService: RoomFacilityMasterServiceProxy
  ) {
    super(injector);
  }

  ngOnInit(): void {
    if (this.id) {
      this._roomTypeService.get(this.id).subscribe(res => {
        this.roomType = res;
        this.cd.detectChanges();
      });
    }
    this._roomFacilityService.getAllRoomFacilityByTenantID(abp.session.tenantId).subscribe(res => {
      this.facilityOptions = res.items.map(f => ({
        label: f.facilityName,
        value: f.id
      }));
    });
  
    if (this.id) {
      this._roomTypeService.get(this.id).subscribe(res => {
        this.roomType = res;
        this.cd.detectChanges();
      });
    }
  }

  save(): void {
    if (!this.isFormValid) {
      this.message.warn('Please enter a room type name.');
      return;
    }
    this.saving = true;
    const input = new CreateUpdateRoomTypeMasterDto();
    input.id = this.id ?? 0;
    input.tenantId = this.appSession.tenantId;
    input.typeName = this.roomType.typeName;
    input.description = this.roomType.description;
    input.defaultPricePerDay = this.roomType.defaultPricePerDay;
    input.facilityIds = this.roomType.facilityIds || [];


    const request = this.id
      ? this._roomTypeService.update(input)
      : this._roomTypeService.create(input);

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
