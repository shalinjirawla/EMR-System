import {
  Component, Injector, OnInit, ViewChild, Output, EventEmitter,
  ChangeDetectorRef
} from '@angular/core';
import { NgForm, FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { BsModalRef } from 'ngx-bootstrap/modal';
import { AppComponentBase } from '@shared/app-component-base';
import {
  RoomServiceProxy,
  RoomTypeMasterServiceProxy,
  RoomDto,
  CreateUpdateRoomDto,
  RoomStatus
} from '@shared/service-proxies/service-proxies';
import { AbpModalHeaderComponent } from '@shared/components/modal/abp-modal-header.component';
import { AbpModalFooterComponent } from '@shared/components/modal/abp-modal-footer.component';
import { SelectModule } from 'primeng/select';
import { InputTextModule } from 'primeng/inputtext';
import { ButtonModule } from 'primeng/button';

@Component({
  selector: 'app-edit-room',
  imports: [
    CommonModule,FormsModule,AbpModalHeaderComponent,AbpModalFooterComponent,SelectModule,InputTextModule,ButtonModule
  ],
  providers: [RoomServiceProxy, RoomTypeMasterServiceProxy],
  templateUrl: './edit-room.component.html',
  styleUrl: './edit-room.component.css'
})
export class EditRoomComponent extends AppComponentBase implements OnInit {
  @Output() onSave = new EventEmitter<any>();
  @ViewChild('editRoomForm', { static: true }) editRoomForm!: NgForm;
  id!: number;

  saving = false;

  room!: CreateUpdateRoomDto;   
  roomTypeOptions: any[] = [];

  statusOptions = [
    { label: 'Available',          value: RoomStatus._0 },
    { label: 'Occupied',           value: RoomStatus._1 },
    { label: 'Reserved',           value: RoomStatus._2 },
    { label: 'Maintenance',        value: RoomStatus._3 }
  ];

  constructor(
    injector: Injector,
    public bsModalRef: BsModalRef,
    private _roomSvc: RoomServiceProxy,
    private _roomTypeSvc: RoomTypeMasterServiceProxy,
    public cd: ChangeDetectorRef,
  ) {
    super(injector);
  }

  ngOnInit(): void {
    this.loadRoomTypes();
    this.fillForm();
    this.cd.detectChanges();
  }


  private loadRoomTypes(): void {
    this._roomTypeSvc.getAllRoomTypeByTenantID(abp.session.tenantId)
      .subscribe(res => this.roomTypeOptions = res.items);

  }

 private fillForm(): void {
  this._roomSvc.getRoomDetailsById(this.id)
      .subscribe(dto => {
        this.room = dto;  
      this.cd.detectChanges();
      });
}

  save(): void {
    if (!this.editRoomForm.valid) { return; }
    this.saving = true;

     this._roomSvc.update(this.room).subscribe({
    next: () => {
      this.saving = false;
      this.notify.info(this.l('SavedSuccessfully'));
      this.bsModalRef.hide();  // Close modal first
      this.onSave.emit();      // Then emit event
      this.cd.detectChanges(); // Finally detect changes
    },
    error: () => this.saving = false
  });
  }
}