import {
  Component, Injector, OnInit, EventEmitter, Output,
  ChangeDetectorRef, ViewChild
} from '@angular/core';
import { NgForm, FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { BsModalRef } from 'ngx-bootstrap/modal';

import {
  CreateUpdateDoctorMasterDto,
  DoctorMasterServiceProxy,
  DoctorDto,
  DoctorServiceProxy
} from '../../../shared/service-proxies/service-proxies';

import { AppComponentBase } from '../../../shared/app-component-base';
import { AbpModalHeaderComponent } from '../../../shared/components/modal/abp-modal-header.component';
import { AbpModalFooterComponent } from '../../../shared/components/modal/abp-modal-footer.component';
import { MultiSelectModule } from 'primeng/multiselect';
import { InputTextModule } from 'primeng/inputtext';
import { ButtonModule } from 'primeng/button';

@Component({
  selector: 'app-createupdate-doctor-master',
  templateUrl: './createupdate-doctor-master.component.html',
  styleUrls: ['./createupdate-doctor-master.component.css'],
  standalone: true,
  imports: [FormsModule,CommonModule,AbpModalHeaderComponent,AbpModalFooterComponent,MultiSelectModule,
    InputTextModule,ButtonModule],
  providers: [DoctorMasterServiceProxy, DoctorServiceProxy]
})
export class CreateupdateDoctorMasterComponent extends AppComponentBase implements OnInit {
  @ViewChild('doctorMasterForm', { static: true }) doctorMasterForm: NgForm;
  @Output() onSave = new EventEmitter<void>();

  saving = false;
  id?: number;

  doctors: DoctorDto[] = [];
  selectedDoctorIds: number[] = [];
  fee: number;

  get isFormValid(): boolean {
    return this.doctorMasterForm?.form?.valid && this.selectedDoctorIds.length > 0;
  }

  constructor(
    injector: Injector,
    public bsModalRef: BsModalRef,
    private cd: ChangeDetectorRef,
    private _doctorService: DoctorServiceProxy,
    private _doctorMasterService: DoctorMasterServiceProxy
  ) {
    super(injector);
  }

  ngOnInit(): void {
    this._doctorService.getAllDoctors().subscribe(res => {
      this.doctors = res.items;
      this.cd.detectChanges();
    });
  
    if (this.id) {
      this._doctorMasterService.get(this.id).subscribe(res => {
        this.selectedDoctorIds = [res.doctorId]; // wrap in array for MultiSelect
        this.fee = res.fee;
        this.cd.detectChanges();
      });
    }
  }
  

  save(): void {
    if (!this.isFormValid) {
      this.message.warn('Please fill all required fields.');
      return;
    }
  
    this.saving = true;
  
    if (this.id) {
      // Single update
      const input = new CreateUpdateDoctorMasterDto();
      input.id = this.id;
      input.tenantId = this.appSession.tenantId;
      input.doctorId = this.selectedDoctorIds[0]; // only one doctor in edit mode
      input.fee = this.fee;
  
      this._doctorMasterService.update(input).subscribe({
        next: () => {
          this.notify.info(this.l('SavedSuccessfully'));
          this.bsModalRef.hide();
          this.onSave.emit();
        },
        error: () => {
          this.saving = false;
        }
      });
    } else {
      // Bulk create
      const requests = this.selectedDoctorIds.map(doctorId => {
        const input = new CreateUpdateDoctorMasterDto();
        input.id = 0;
        input.tenantId = this.appSession.tenantId;
        input.doctorId = doctorId;
        input.fee = this.fee;
  
        return this._doctorMasterService.create(input).toPromise();
      });
  
      Promise.all(requests)
        .then(() => {
          this.notify.info(this.l('SavedSuccessfully'));
          this.bsModalRef.hide();
          this.onSave.emit();
        })
        .catch(() => {
          this.saving = false;
        });
    }
  }
  
}
