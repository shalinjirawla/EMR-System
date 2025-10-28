import { Component, Injector, OnInit, EventEmitter, Output, ViewChild, ChangeDetectorRef } from '@angular/core';
import { NgForm, FormsModule } from '@angular/forms';
import { BsModalRef } from 'ngx-bootstrap/modal';
import { CommonModule } from '@angular/common';
import { DropdownModule } from 'primeng/dropdown';
import { InputTextModule } from 'primeng/inputtext';
import { CheckboxModule } from 'primeng/checkbox';
import { TextareaModule } from 'primeng/textarea';
import { AbpModalHeaderComponent } from '../../../shared/components/modal/abp-modal-header.component';
import { AbpModalFooterComponent } from '../../../shared/components/modal/abp-modal-footer.component';
import { AppComponentBase } from '@shared/app-component-base';
import {
  DeathRecordServiceProxy,
  PatientServiceProxy,
  DoctorServiceProxy,
  NurseServiceProxy,
  CreateUpdateDeathRecordDto,
  DeathRecordDto,
  PatientDropDownDto,
  DoctorDto,
  NurseDto
} from '@shared/service-proxies/service-proxies';
import { DatePickerModule } from 'primeng/datepicker';

@Component({
  selector: 'app-edit-death-record',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    DropdownModule,
    InputTextModule,
    DatePickerModule,
    CheckboxModule,
    TextareaModule,
    AbpModalHeaderComponent,
    AbpModalFooterComponent
  ],
  providers: [DeathRecordServiceProxy, PatientServiceProxy, DoctorServiceProxy, NurseServiceProxy],
  templateUrl: './edit-death-record.component.html',
  styleUrl: './edit-death-record.component.css'
})
export class EditDeathRecordComponent extends AppComponentBase implements OnInit {

  @ViewChild('editDeathForm', { static: true }) editDeathForm: NgForm;
  @Output() onSave = new EventEmitter<void>();

  saving = false;
  deathRecordId: number;
  deathRecord: CreateUpdateDeathRecordDto = new CreateUpdateDeathRecordDto();
  today: Date = new Date();

  patientList: PatientDropDownDto[] = [];
  doctorList: DoctorDto[] = [];
  nurseList: NurseDto[] = [];

  constructor(
    injector: Injector,
    public bsModalRef: BsModalRef,
    private _deathRecordService: DeathRecordServiceProxy,
    private _patientService: PatientServiceProxy,
    private _doctorService: DoctorServiceProxy,
    private _nurseService: NurseServiceProxy,
    private cd: ChangeDetectorRef
  ) {
    super(injector);
  }

  ngOnInit(): void {
    this.loadDropdowns();
    this.loadRecord();
  }

  loadDropdowns() {
    this._patientService.patientDropDown().subscribe(res => {
      this.patientList = res;
      this.cd.detectChanges();
    });

    this._doctorService.getAllDoctors().subscribe(res => {
      this.doctorList = res.items;
      this.cd.detectChanges();
    });

    this._nurseService.getAllNursesByTenantID(abp.session.tenantId).subscribe(res => {
      this.nurseList = res.items;
      this.cd.detectChanges();
    });
  }

  loadRecord() {
    if (!this.deathRecordId) return;

    this._deathRecordService.get(this.deathRecordId).subscribe((res: DeathRecordDto) => {
      this.deathRecord = DeathRecordDto.fromJS(res);

      (this.deathRecord as any).deathDate = this.deathRecord.deathDate
        ? this.deathRecord.deathDate.toDate()
        : null;

      (this.deathRecord as any).deathTime = this.deathRecord.deathTime
        ? this.deathRecord.deathTime.toDate()
        : null;

      this.cd.detectChanges();
    });
  }

  get isFormValid(): boolean {
    return this.editDeathForm?.form?.valid;
  }

  save(): void {
    if (!this.isFormValid) {
      this.message.warn('Please complete the form properly.');
      return;
    }

    this.saving = true;

    this._deathRecordService.update(this.deathRecord).subscribe({
      next: () => {
        this.notify.info(this.l('UpdatedSuccessfully'));
        this.bsModalRef.hide();
        this.onSave.emit();
      },
      error: () => {
        this.saving = false;
      }
    });
  }
}
