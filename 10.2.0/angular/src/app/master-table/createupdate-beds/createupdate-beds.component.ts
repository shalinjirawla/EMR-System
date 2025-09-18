import {
  Component,
  Injector,
  OnInit,
  EventEmitter,
  Output,
  ViewChild,
  ChangeDetectorRef,
} from '@angular/core';
import { NgForm, FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { BsModalRef } from 'ngx-bootstrap/modal';
import { AppComponentBase } from '@shared/app-component-base';
import { AbpModalHeaderComponent } from '@shared/components/modal/abp-modal-header.component';
import { AbpModalFooterComponent } from '@shared/components/modal/abp-modal-footer.component';
import { InputTextModule } from 'primeng/inputtext';
import { CheckboxModule } from 'primeng/checkbox';
import { DropdownModule } from 'primeng/dropdown';
import {
  BedServiceProxy,
  BedDto,
  CreateUpdateBedDto,
  RoomServiceProxy,
  RoomDto,
} from '@shared/service-proxies/service-proxies';

@Component({
  selector: 'app-createupdate-beds',
   standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    InputTextModule,
    CheckboxModule,
    AbpModalHeaderComponent,
    AbpModalFooterComponent,
    DropdownModule
  ],
  providers: [BedServiceProxy, RoomServiceProxy],
  templateUrl: './createupdate-beds.component.html',
  styleUrl: './createupdate-beds.component.css'
})
export class CreateupdateBedsComponent extends AppComponentBase implements OnInit {
  @ViewChild('bedForm', { static: true }) bedForm: NgForm;
  @Output() onSave = new EventEmitter<void>();

  saving = false;
  id?: number;

  // Dropdown data
  rooms: RoomDto[] = [];
  selectedRoomId?: number;

  // Update mode
  bed: Partial<BedDto> = { bedNumber: ''};
  isActive = true;

  // Bulk create fields
  prefix: string = '';
  startNumber: number;
  count: number;

  constructor(
    injector: Injector,
    public bsModalRef: BsModalRef,
    private _bedService: BedServiceProxy,
    private _roomService: RoomServiceProxy,
    private cd: ChangeDetectorRef
  ) {
    super(injector);
  }

  ngOnInit(): void {
    this._roomService.getAllRooms(this.appSession.tenantId).subscribe((res) => {
      this.rooms = res;
    });

    if (this.id) {
      this._bedService.get(this.id).subscribe((res) => {
        this.bed = res;
        this.selectedRoomId = res.roomId!;
        this.cd.detectChanges();
      });
    }
  }

  get isFormValid(): boolean {
    if (this.id) {
      return this.bedForm?.form.valid && !!this.bed.bedNumber;
    } else {
      return this.bedForm?.form.valid && this.prefix && this.count > 0 && this.startNumber >= 0;
    }
  }

  save(): void {
    if (!this.isFormValid) {
      this.message.warn('Please fill in required fields.');
      return;
    }

    this.saving = true;

    if (this.id) {
      // 🔹 Update Bed
      const input = new CreateUpdateBedDto();
      input.id = this.id;
      input.tenantId = this.appSession.tenantId;
      input.bedNumber = this.bed.bedNumber!;
      input.roomId = this.selectedRoomId!;

      this._bedService.update(input).subscribe({
        next: () => {
          this.notify.success(this.l('SavedSuccessfully'));
          this.bsModalRef.hide();
          this.onSave.emit();
        },
        error: () => (this.saving = false),
      });
    } else {
      // 🔹 Bulk Create Beds
      const inputs = Array.from({ length: this.count }, (_, i) => {
        const dto = new CreateUpdateBedDto();
        dto.id = 0;
        dto.tenantId = this.appSession.tenantId;
        dto.bedNumber = `${this.prefix}${this.startNumber + i}`;
        dto.roomId = this.selectedRoomId!;
        return dto;
      });
      this._bedService.createBulkBeds(inputs).subscribe({
        next: () => {
          this.notify.success(this.l('SavedSuccessfully'));
          this.bsModalRef.hide();
          this.onSave.emit();
        },
        error: () => (this.saving = false),
        complete: () => (this.saving = false),
      });
    }
  }
}