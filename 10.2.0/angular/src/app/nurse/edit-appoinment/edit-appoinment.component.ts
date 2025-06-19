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
import { AppointmentDto, AppointmentServiceProxy, AppointmentStatus, CreateUpdatePrescriptionDto, CreateUpdatePrescriptionItemDto, DoctorDto, DoctorServiceProxy, NurseDto, NurseServiceProxy, PatientDto, PatientServiceProxy, PrescriptionServiceProxy } from '@shared/service-proxies/service-proxies';
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
  saving = false;
  appointment: any = {
    patientId: null,
    doctorId: null,
    nurseId: null,
    status: null,
    isFollowUp: false,
    appointmentDate: null,
    startTime: null,
    endTime: null,
    reason: ''
  };
  patients!: PatientDto[];
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
    this.LoadDoctors();
    this.LoadNurse();
    this.LoadPatients();
    this.LoadStatus();
    this.FillForm();
  }
  FillForm() {
    debugger
    this._appointmentService.getAppointmentDetailsById(this.id).subscribe((result) => {
      debugger
      this.appointment.patientId=result.patientId;
      this.appointment.doctorId=result.doctorId;
      this.appointment.nurseId=result.nurseId;
      this.appointment.status=result.status;
      this.appointment.isFollowUp=result.isFollowUp;
      this.appointment.appointmentDate=result.appointmentDate;
      // this.appointment.startTime=result.patient.id;
      // this.appointment.endTime=result.patient.id;
      this.appointment.reason=result.reasonForVisit;
      this.cd.detectChanges();
    });
  }
  LoadPatients() {
    this._patientService.getAllPatientByTenantID(abp.session.tenantId).subscribe({
      next: (res) => {
        this.patients = res.items;
      }, error: (err) => {
      }
    })
  }
  LoadDoctors() {
    this._doctorService.getAllDoctorsByTenantID(abp.session.tenantId).subscribe({
      next: (res) => {
        this.doctors = res.items;
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
  }
  LoadNurse() {
    this._nurseService.getAllNursesByTenantID(abp.session.tenantId).subscribe({
      next: (res) => {
        this.nurse = res.items;
      }, error: (err) => {
      }
    })
  }
  save() { }
}
