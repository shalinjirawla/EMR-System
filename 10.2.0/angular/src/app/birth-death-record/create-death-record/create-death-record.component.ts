import { Component, Injector, OnInit, EventEmitter, Output, ViewChild } from '@angular/core';
import { NgForm, FormsModule } from '@angular/forms';
import { BsModalRef } from 'ngx-bootstrap/modal';
import { CommonModule } from '@angular/common';
import { DropdownModule } from 'primeng/dropdown';
import { InputTextModule } from 'primeng/inputtext';
import { CalendarModule } from 'primeng/calendar';
import { CheckboxModule } from 'primeng/checkbox';
import { TextareaModule } from 'primeng/textarea';
import { AbpModalHeaderComponent } from '../../../shared/components/modal/abp-modal-header.component';
import { AbpModalFooterComponent } from '../../../shared/components/modal/abp-modal-footer.component';
import { AppComponentBase } from '@shared/app-component-base';
import {
  DeathRecordServiceProxy,
  CreateUpdateDeathRecordDto,
  PatientServiceProxy,
  DoctorServiceProxy,
  NurseServiceProxy,
  PatientDropDownDto,
  DoctorDto,
  NurseDto
} from '@shared/service-proxies/service-proxies';
import { DatePickerModule } from 'primeng/datepicker';
import moment from 'moment';

@Component({
  selector: 'app-create-death-record',
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
  templateUrl: './create-death-record.component.html',
  styleUrl: './create-death-record.component.css'
})
export class CreateDeathRecordComponent extends AppComponentBase implements OnInit {

  @ViewChild('deathRecordForm', { static: true }) deathRecordForm: NgForm;
  @Output() onSave = new EventEmitter<void>();
  saving = false;
  today: Date = new Date();

  deathRecord: CreateUpdateDeathRecordDto = new CreateUpdateDeathRecordDto();

  // Dropdown data
  patientList: PatientDropDownDto[] = [];
  doctorList: DoctorDto[] = [];
  nurseList: NurseDto[] = [];

  constructor(
    injector: Injector,
    public bsModalRef: BsModalRef,
    private _deathRecordService: DeathRecordServiceProxy,
    private _patientService: PatientServiceProxy,
    private _doctorService: DoctorServiceProxy,
    private _nurseService: NurseServiceProxy
  ) {
    super(injector);
    this.deathRecord.tenantId = abp.session.tenantId;
  }

  ngOnInit(): void {
    this.loadDropdowns();
  }

  loadDropdowns() {
    this._patientService.patientDropDown().subscribe(res => {
      this.patientList = res;
    });

    this._doctorService.getAllDoctors().subscribe(res => {
      this.doctorList = res.items;
    });

    this._nurseService.getAllNursesByTenantID(abp.session.tenantId).subscribe(res => {
      this.nurseList = res.items;
    });
  }

  get isFormValid(): boolean {
    return this.deathRecordForm?.form?.valid;
  }

  save(): void {
    if (!this.isFormValid) {
      this.message.warn('Please complete the form properly.');
      return;
    }

    // Convert date/time fields
    if (this.deathRecord.deathDate instanceof Date) {
      this.deathRecord.deathDate = moment(this.deathRecord.deathDate);
    }

    if (this.deathRecord.deathTime instanceof Date) {
      this.deathRecord.deathTime = moment(this.deathRecord.deathTime);
    }

    this.saving = true;
    this._deathRecordService.create(this.deathRecord).subscribe({
      next: () => {
        this.notify.info(this.l('SavedSuccessfully'));
        this.bsModalRef.hide();
        this.onSave.emit();
      },
      error: () => {
        this.saving = false;
      },
    });
  }
}
