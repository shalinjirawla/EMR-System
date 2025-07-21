import {
  Component, Injector, ViewChild, OnInit, EventEmitter, Output
} from '@angular/core';
import { NgForm, FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { AppComponentBase } from '@shared/app-component-base';
import {
  RoomTypeMasterServiceProxy,
  RoomServiceProxy,
  CreateUpdateRoomDto,
  RoomStatus
} from '@shared/service-proxies/service-proxies';
import { BsModalRef } from 'ngx-bootstrap/modal';
import { AbpModalHeaderComponent } from '@shared/components/modal/abp-modal-header.component';
import { AbpModalFooterComponent } from '@shared/components/modal/abp-modal-footer.component';
import { ButtonModule } from 'primeng/button';
import { SelectModule } from 'primeng/select';
import { InputTextModule } from 'primeng/inputtext';
import { AutoCompleteModule } from 'primeng/autocomplete';

@Component({
  selector: 'app-add-room',
  imports: [FormsModule, AbpModalHeaderComponent, AbpModalFooterComponent,SelectModule,InputTextModule,ButtonModule,AutoCompleteModule],
  providers:[RoomTypeMasterServiceProxy,RoomServiceProxy],
  templateUrl: './add-room.component.html',
  styleUrl: './add-room.component.css'
})
export class AddRoomComponent extends AppComponentBase implements OnInit {
  @ViewChild('createRoomModal', { static: true }) form!: NgForm;
  @Output() onSave = new EventEmitter<void>();

  saving = false;
  room: CreateUpdateRoomDto = {
    id: 0,
    tenantId: abp.session.tenantId,
    roomNumber: '',
    floor: 0,
    roomTypeMasterId: 0,
    status: RoomStatus._0
  } as CreateUpdateRoomDto;

  selectedRoomNos: string[] = [];
  filteredRoomNos: string[] = [];
  roomNosMasterList: string[] = [];
  status: RoomStatus = RoomStatus._0;
  roomTypeOptions: any[] = [];

  statusOptions = [
    { label: 'Available', value: RoomStatus._0 },
    { label: 'Occupied', value: RoomStatus._1 },
    { label: 'Reserved', value: RoomStatus._2 },
    { label: 'Under Maintenance', value: RoomStatus._3 }
  ];

  constructor(
    injector: Injector,
    public bsModalRef: BsModalRef,
    private _roomTypeSvc: RoomTypeMasterServiceProxy,
    private _roomSvc: RoomServiceProxy
  ) {
    super(injector);
  }

  ngOnInit(): void {
    this.loadRoomTypes();
  }

  private loadRoomTypes(): void {
    this._roomTypeSvc.getAllRoomTypeByTenantID(abp.session.tenantId)
      .subscribe(res => this.roomTypeOptions = res.items);
  }
  filterRoomNos(event: any): void {
    const query = event.query.toLowerCase();
    this.filteredRoomNos = this.roomNosMasterList.filter((room) =>
      room.toLowerCase().includes(query)
    );
  }
  generateRoomNumbers(start: number, end: number): string[] {
    const list: string[] = [];
    for (let i = start; i <= end; i++) {
      list.push(i.toString());
    }
    return list;
  }


  // save(): void {
  //   if (this.form.invalid) {
  //     this.message.warn('Please fill form correctly');
  //     return;
  //   }
  //   this.saving = true;

  //   this._roomSvc.create(this.room).subscribe({
  //     next: () => {
  //       this.notify.success(this.l('SavedSuccessfully'));
  //       this.bsModalRef.hide();
  //       this.onSave.emit();
  //     },
  //     error: () => this.saving = false,
  //     complete: () => this.saving = false
  //   });
  // }

  save(): void {
    debugger
    if (
      !this.selectedRoomNos.length ||
      !this.room.floor ||
      !this.room.roomTypeMasterId
    ) {
      this.message.warn('Please fill all required fields.');
      return;
    }

    this.saving = true;

    const bulkRooms: CreateUpdateRoomDto[] = this.selectedRoomNos.map(
      (roomNo) =>
        ({
          roomNumber: roomNo,
          floor: this.room.floor,
          roomTypeMasterId: this.room.roomTypeMasterId,
          status: this.status,
          tenantId: abp.session.tenantId
        } as CreateUpdateRoomDto)
    );
    debugger;
    this._roomSvc.createBulkRooms(bulkRooms).subscribe({
      next: () => {
        this.notify.success(this.l('SavedSuccessfully'));
        this.bsModalRef.hide();
        this.onSave.emit();
      },
      error: () => {
        this.saving = false;
      },
      complete: () => {
        this.saving = false;
      }
    });
  }
}
