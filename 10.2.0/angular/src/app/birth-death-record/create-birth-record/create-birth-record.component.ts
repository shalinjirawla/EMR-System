import { Component, Injector, OnInit, EventEmitter, Output, ViewChild } from '@angular/core';
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
import { BirthRecordServiceProxy, PatientServiceProxy, DoctorServiceProxy, NurseServiceProxy, CreateUpdateBirthRecordDto, PatientDropDownDto, DoctorDto, NurseDto, GenderType, BirthType, DeliveryType } from '@shared/service-proxies/service-proxies';
import { DatePickerModule } from 'primeng/datepicker';
import moment from 'moment';
@Component({
  selector: 'app-create-birth-record',
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

  templateUrl: './create-birth-record.component.html',
  styleUrl: './create-birth-record.component.css'
})
export class CreateBirthRecordComponent extends AppComponentBase implements OnInit {

  @ViewChild('birthRecordForm', { static: true }) birthRecordForm: NgForm;
  @Output() onSave = new EventEmitter<void>();
  saving = false;
  today: Date = new Date();

  birthRecord: CreateUpdateBirthRecordDto = new CreateUpdateBirthRecordDto();

  // Dropdown Data
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
    private _nurseService: NurseServiceProxy
  ) {
    super(injector);
    this.birthRecord.tenantId = abp.session.tenantId;
  }

  ngOnInit(): void {
    this.loadDropdowns();
  }

  loadDropdowns() {
    this._patientService.patientDropDown().subscribe(res => {
      this.motherList = res;
    });

    this._doctorService.getAllDoctors().subscribe(res => {
      this.doctorList = res.items;
    });

    this._nurseService.getAllNursesByTenantID(abp.session.tenantId).subscribe(res => {
      this.nurseList = res.items;
    });
  }

  get isFormValid(): boolean {
    return this.birthRecordForm?.form?.valid;
  }
  save(): void {
    if (!this.isFormValid) {
      this.message.warn('Please complete the form properly.');
      return;
    }

    // ✅ If birthTime is a Date (from p-datepicker timeOnly), convert it to Moment
    if (this.birthRecord.birthTime instanceof Date) {
      this.birthRecord.birthTime = moment(this.birthRecord.birthTime);
    }

    // ✅ If birthTime is a string (manually edited), parse it
    else if (typeof (this.birthRecord.birthTime as any) === 'string' && (this.birthRecord.birthTime as any).length > 0) {

      const parsed = moment(this.birthRecord.birthTime, ['hh:mm A', 'HH:mm']);
      if (parsed.isValid()) {
        this.birthRecord.birthTime = parsed;
      } else {
        this.message.warn('Invalid birth time format.');
        return;
      }
    }

    // ✅ Same check for BirthDate if you’re using p-datepicker
    if (this.birthRecord.birthDate instanceof Date) {
      this.birthRecord.birthDate = moment(this.birthRecord.birthDate);
    }

    this.saving = true;

    this._birthRecordService.create(this.birthRecord).subscribe({
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
