import { Component, Injector, OnInit, EventEmitter, Output, ViewChild, ChangeDetectorRef } from '@angular/core';
import { NgForm, FormsModule } from '@angular/forms';
import { BsModalRef } from 'ngx-bootstrap/modal';
import { CommonModule } from '@angular/common';
import { DropdownModule } from 'primeng/dropdown';
import { InputTextModule } from 'primeng/inputtext';
import { InputNumberModule } from 'primeng/inputnumber';
import { CalendarModule } from 'primeng/calendar';
import { CheckboxModule } from 'primeng/checkbox';
import { TextareaModule } from 'primeng/textarea';
import { AbpModalHeaderComponent } from '../../../shared/components/modal/abp-modal-header.component';
import { AbpModalFooterComponent } from '../../../shared/components/modal/abp-modal-footer.component';
import { AppComponentBase } from '@shared/app-component-base';
import {
  BirthRecordServiceProxy,
  PatientServiceProxy,
  DoctorServiceProxy,
  NurseServiceProxy,
  CreateUpdateBirthRecordDto,
  BirthRecordDto,
  PatientDropDownDto,
  DoctorDto,
  NurseDto,
  GenderType,
  BirthType,
  DeliveryType
} from '@shared/service-proxies/service-proxies';
import { DatePickerModule } from 'primeng/datepicker';

@Component({
  selector: 'app-edit-birth-record',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    DropdownModule,
    InputTextModule,
    InputNumberModule,
    DatePickerModule,
    CheckboxModule,
    TextareaModule,
    AbpModalHeaderComponent,
    AbpModalFooterComponent
  ],
  providers: [BirthRecordServiceProxy, PatientServiceProxy, DoctorServiceProxy, NurseServiceProxy],
  templateUrl: './edit-birth-record.component.html',
  styleUrl: './edit-birth-record.component.css'
})
export class EditBirthRecordComponent extends AppComponentBase implements OnInit {
  @ViewChild('editBirthForm', { static: true }) editBirthForm: NgForm;
  @Output() onSave = new EventEmitter<void>();

  saving = false;
  birthRecordId: number; // passed from parent
  birthRecord: CreateUpdateBirthRecordDto = new CreateUpdateBirthRecordDto();
  today: Date = new Date();
  isFutureDate: boolean = false;
  minTime: Date;
  maxTime: Date;

  motherList: PatientDropDownDto[] = [];
  doctorList: DoctorDto[] = [];
  nurseList: NurseDto[] = [];

  genderOptions = [
    { label: 'Male', value: GenderType._1 },
    { label: 'Female', value: GenderType._2 },
    { label: 'Other', value: GenderType._3 }
  ];

  birthTypeOptions = [
    { label: 'Single', value: BirthType._1 },
    { label: 'Twin', value: BirthType._2 },
    { label: 'Triplet', value: BirthType._3 },
    { label: 'Multiple', value: BirthType._4 }
  ];

  deliveryTypeOptions = [
    { label: 'Normal', value: DeliveryType._1 },
    { label: 'C-Section', value: DeliveryType._2 }
  ];

  constructor(
    injector: Injector,
    public bsModalRef: BsModalRef,
    private _birthRecordService: BirthRecordServiceProxy,
    private _patientService: PatientServiceProxy,
    private _doctorService: DoctorServiceProxy,
    private _nurseService: NurseServiceProxy,
    public cd: ChangeDetectorRef
  ) {
    super(injector);
  }

  ngOnInit(): void {
    this.loadDropdowns();
    this.loadRecord();
  }
  loadDropdowns() {
    this._patientService.patientDropDown().subscribe(res => {
      this.motherList = res;
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
    if (!this.birthRecordId) return;

    this._birthRecordService.get(this.birthRecordId).subscribe((res: BirthRecordDto) => {
      this.birthRecord = BirthRecordDto.fromJS(res);

      // ✅ Cast with "as any" to bypass Moment type check
      (this.birthRecord as any).birthDate = this.birthRecord.birthDate
        ? this.birthRecord.birthDate.toDate()
        : null;

      (this.birthRecord as any).birthTime = this.birthRecord.birthTime
        ? this.birthRecord.birthTime.toDate()
        : null;

      this.cd.detectChanges();
    });
  }


  get isFormValid(): boolean {
    return this.editBirthForm?.form?.valid;
  }

  save(): void {
    if (!this.isFormValid) {
      this.message.warn('Please complete the form properly.');
      return;
    }


    this.saving = true;

    this._birthRecordService.update(this.birthRecord).subscribe({
      next: () => {
        this.notify.info(this.l('UpdatedSuccessfully'));
        this.onSave.emit();
        this.bsModalRef.hide();
      },
      error: () => {
        this.saving = false;
      }
    });
  }
}
