import { Component, Injector, OnInit, EventEmitter, Output, ChangeDetectorRef, ViewChild } from '@angular/core';
import { BsModalRef } from 'ngx-bootstrap/modal';
import { forEach as _forEach, includes as _includes, map as _map } from 'lodash-es';
import { AppComponentBase } from '@shared/app-component-base';
import { FormsModule, NgForm } from '@angular/forms';
import { AbpModalHeaderComponent } from '../../../shared/components/modal/abp-modal-header.component';
import { AbpValidationSummaryComponent } from '../../../shared/components/validation/abp-validation.summary.component';
import { AbpModalFooterComponent } from '../../../shared/components/modal/abp-modal-footer.component';
import { LocalizePipe } from '@shared/pipes/localize.pipe';
import { CommonModule } from '@node_modules/@angular/common';
import { AppointmentDto, AppointmentServiceProxy, AppointmentStatus, CreateUpdateAppointmentDto, CreateUpdatePrescriptionDto, CreateUpdatePrescriptionItemDto, DoctorDto, DoctorServiceProxy, NurseDto, NurseServiceProxy, PatientDropDownDto, PatientDto, PatientServiceProxy, PrescriptionServiceProxy } from '@shared/service-proxies/service-proxies';
import moment from 'moment';
import { TextareaModule } from 'primeng/textarea';
import { SelectModule } from 'primeng/select';
import { CalendarModule } from 'primeng/calendar';
import { DropdownModule } from 'primeng/dropdown';
import { CheckboxModule } from 'primeng/checkbox';
import { InputTextModule } from 'primeng/inputtext';
import { ButtonModule } from 'primeng/button';
import { DatePickerModule } from 'primeng/datepicker';

@Component({
  selector: 'app-edit-appoinment',
  imports: [
    FormsModule, AbpModalHeaderComponent,
    AbpModalFooterComponent, CommonModule,
    CalendarModule, DropdownModule, CheckboxModule, InputTextModule, TextareaModule, DatePickerModule,
    ButtonModule, CommonModule, SelectModule, AbpModalHeaderComponent, AbpModalFooterComponent
  ],
  templateUrl: './edit-appoinment.component.html',
  styleUrl: './edit-appoinment.component.css',
  providers: [NurseServiceProxy, PatientServiceProxy, AppointmentServiceProxy, DoctorServiceProxy],
})
export class EditAppoinmentComponent extends AppComponentBase implements OnInit {
  @Output() onSave = new EventEmitter<any>();
  @ViewChild('editAppoinmentForm', { static: true }) editAppoinmentForm: NgForm;
  id: number;
  status: AppointmentStatus;
  saving = false;
  appointment: any = {
    id: null,
    patientId: null,
    doctorId: null,
    nurseId: null,
    status: null,
    isFollowUp: false,
    appointmentDate: null,
    startTime: null,
    endTime: null,
    reasonForVisit: '',
    tenantId: 0,
  };
  patients!: PatientDropDownDto[];
  nurse!: NurseDto[];
  doctors!: DoctorDto[];
  statusOptions!: any[];

  constructor(
    injector: Injector,
    public bsModalRef: BsModalRef,
    private cd: ChangeDetectorRef,
    private _patientService: PatientServiceProxy,
    private _appointmentService: AppointmentServiceProxy,
    private _doctorService: DoctorServiceProxy,
    private _nurseService: NurseServiceProxy,
  ) { super(injector); }

  get isFormValid(): boolean {
    const mainFormValid = this.editAppoinmentForm?.form?.valid;
    return mainFormValid;
  }

  ngOnInit(): void {
    if (this.status) {
      this.appointment.status = this.status;
    }
    this.LoadDoctors();
    this.LoadNurse();
    this.LoadPatients();
    this.LoadStatus();
    this.FillForm();
  }
  FillForm() {
    this._appointmentService.getAppointmentDetailsById(this.id).subscribe((result) => {
      this.appointment.id = result.id;
      this.appointment.tenantId = result.tenantId;
      this.appointment.patientId = result.patientId;
      this.appointment.doctorId = result.doctorId;
      this.appointment.nurseId = result.nurseId;
      this.appointment.status = result.status;
      this.appointment.isFollowUp = result.isFollowUp;
      this.appointment.appointmentDate = result.appointmentDate ? moment(result.appointmentDate).toDate() : null;
      this.appointment.startTime = result.startTime ? this.convertTimeToDate(result.startTime) : null;
      this.appointment.endTime = result.endTime ? this.convertTimeToDate(result.endTime) : null;
      this.appointment.reasonForVisit = result.reasonForVisit;
      this.cd.detectChanges();
    });
  }
  LoadPatients() {
    this._patientService.patientDropDown().subscribe({
      next: (res) => {
        this.patients = res;
        this.cd.detectChanges();
      }, error: (err) => {
      }
    })
  }
  LoadDoctors() {
    this._doctorService.getAllDoctorsByTenantID(abp.session.tenantId).subscribe({
      next: (res) => {
        this.doctors = res.items;
        this.cd.detectChanges();
      }, error: (err) => {
      }
    })
  }
  LoadStatus() {
    this.statusOptions = [
      { label: 'Scheduled', value: AppointmentStatus._0 },
      { label: 'Checked In', value: AppointmentStatus._1 },
      { label: 'Completed', value: AppointmentStatus._2 },
      { label: 'Cancelled', value: AppointmentStatus._3 },
      { label: 'Rescheduled', value: AppointmentStatus._4 },
    ];
    this.cd.detectChanges();
  }
  LoadNurse() {
    this._nurseService.getAllNursesByTenantID(abp.session.tenantId).subscribe({
      next: (res) => {
        this.nurse = res.items;
        this.cd.detectChanges();
      }, error: (err) => {
      }
    })
  }
  convertTimeToDate(timeStr: string): Date {
    const [hours, minutes, seconds] = timeStr.split(':').map(Number);
    const now = new Date();
    now.setHours(hours, minutes, seconds || 0, 0);
    return now;
  }
  isSaveDisabled() {
    if (!this.editAppoinmentForm.valid || this.saving) {
      return true;
    }
  }
  save() {
    this.saving = true;
    if (!this.validateStartEndTime()) {
      this.saving = false;
      return;
    }
    // if (!this.validateAppointmentDate()) {
    //   this.saving = false;
    //   return;
    // }
    this.appointment.startTime = this.dateToTimeString(this.appointment.startTime);
    this.appointment.endTime = this.dateToTimeString(this.appointment.endTime);
    this._appointmentService.updateAppoinment(this.appointment).subscribe({
      next: (res) => {
        this.saving = false;
        this.cd.detectChanges();
        this.notify.info(this.l('SavedSuccessfully'));
        this.bsModalRef.hide();
        this.onSave.emit();
      }, error: (err) => {
        this.cd.detectChanges();
        this.notify.info(this.l('SavedSuccessfully'));
        this.bsModalRef.hide();
        this.onSave.emit();
      }
    })
  }
  validateStartEndTime(): boolean {
    if (!this.appointment.startTime || !this.appointment.endTime) {
      this.message.warn('Please select both Start Time and End Time.');
      return false;
    }

    const start = moment(this.appointment.startTime);
    const end = moment(this.appointment.endTime);

    if (start.isSameOrAfter(end)) {
      this.message.warn('Start Time must be earlier than End Time.');
      return false;
    }

    return true;
  }
  validateAppointmentDate(): boolean {
    if (!this.appointment.appointmentDate) {
      this.message.warn("Please select an appointment date.");
      return false;
    }

    const selectedDate = moment(this.appointment.appointmentDate).startOf('day');
    const today = moment().startOf('day');

    if (!selectedDate.isAfter(today)) {
      this.message.warn("Appointment date must be in the future (not today).");
      return false;
    }

    return true;
  }
  dateToTimeString(date: Date): string {
    const hours = date.getHours().toString().padStart(2, '0');
    const minutes = date.getMinutes().toString().padStart(2, '0');
    const seconds = date.getSeconds().toString().padStart(2, '0');
    return `${hours}:${minutes}:${seconds}`;
  }
}
