import { Component, Injector, OnInit, EventEmitter, Output, ChangeDetectorRef, ViewChild } from '@angular/core';
import { forEach as _forEach, map as _map } from 'lodash-es';
import { FormsModule, NgForm } from '@angular/forms';
import { AbpModalHeaderComponent } from '../../../shared/components/modal/abp-modal-header.component';
import { AbpValidationSummaryComponent } from '../../../shared/components/validation/abp-validation.summary.component';
import { EqualValidator } from '../../../shared/directives/equal-validator.directive';
import { AbpModalFooterComponent } from '../../../shared/components/modal/abp-modal-footer.component';
import { LocalizePipe } from '@shared/pipes/localize.pipe';
import { AppSessionService } from '@shared/session/app-session.service';
import { CommonModule } from '@node_modules/@angular/common';
import { AppComponentBase } from '@shared/app-component-base';
import { BsModalRef, BsModalService } from 'ngx-bootstrap/modal';
import { SelectModule } from 'primeng/select';
import { ButtonModule } from 'primeng/button';
import { AppointmentDto, AppointmentServiceProxy, AppointmentStatus, CreateUpdateAppointmentDto, DoctorDto, DoctorServiceProxy, NurseDto, NurseServiceProxy, PatientDropDownDto, PatientDto, PatientServiceProxy } from '@shared/service-proxies/service-proxies';
import { DatePickerModule } from 'primeng/datepicker';
import { CheckboxModule } from 'primeng/checkbox';
import { TextareaModule } from 'primeng/textarea';
import { CreateUserDialogComponent } from '@app/users/create-user/create-user-dialog.component';
import { PermissionCheckerService } from '@node_modules/abp-ng2-module';
import moment, { Moment } from 'moment';

@Component({
  selector: 'app-create-appoinment',
  imports: [
    FormsModule,
    AbpModalHeaderComponent,
    AbpModalFooterComponent,
    CommonModule, DatePickerModule, TextareaModule,
    SelectModule, ButtonModule, CheckboxModule
  ],
  providers: [DoctorServiceProxy, NurseServiceProxy, PatientServiceProxy, AppointmentServiceProxy, AppSessionService],
  standalone: true,
  templateUrl: './create-appoinment.component.html',
  styleUrl: './create-appoinment.component.css'
})
//extends PagedListingComponentBase<AppointmentDto>
export class CreateAppoinmentComponent extends AppComponentBase implements OnInit {
  @ViewChild('createAppoinmentModal', { static: true }) createAppoinmentModal: NgForm;
  @Output() onSave = new EventEmitter<void>();
  saving = false;
  patients!: PatientDropDownDto[];
  nurse!: NurseDto[];
  doctors!: DoctorDto[];
  statusOptions!: any[];
  showAddPatientButton = false;
  tomorrow!: Date;
  appointment: any = {
    id: 0,
    tenantId: 0,
    patientId: null,
    doctorId: null,
    nurseId: null,
    status: null,
    isFollowUp: false,
    appointmentDate: null,
    startTime: null,
    endTime: null,
    reasonForVisit: '',
  };
  get isFormValid(): boolean {
    const mainFormValid = this.createAppoinmentModal?.form?.valid;
    return mainFormValid;
  }
  constructor(
    injector: Injector,
    public bsModalRef: BsModalRef,
    private cd: ChangeDetectorRef,
    private _doctorService: DoctorServiceProxy,
    private _nurseService: NurseServiceProxy,
    private _patientService: PatientServiceProxy,
    private _sessionService: AppSessionService,
    private _appoinmentService: AppointmentServiceProxy,
    private _modalService: BsModalService,
    private permissionChecker: PermissionCheckerService
  ) {
    super(injector);
  }

  ngOnInit(): void {
    this.tomorrow = moment().add(1, 'day').toDate();
    this.showAddPatientButton = this.permissionChecker.isGranted('Pages.Users');
    this.LoadDoctors();
    this.LoadNurse();
    this.LoadPatients();
    this.LoadStatus();
  }
  LoadPatients() {
    this._patientService.patientDropDown().subscribe({
      next: (res) => {
        this.patients = res;
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
  showCreatePatientDialog(id?: number): void {
    let createOrEditPatientDialog: BsModalRef;
    createOrEditPatientDialog = this._modalService.show(CreateUserDialogComponent, {
      class: 'modal-lg',
      initialState: {
        defaultRole: 'Patient',
        disableRoleSelection: true
      }
    });
    createOrEditPatientDialog.content.onSave.subscribe(() => {
      this.LoadPatients();
    });
  }
  save(): void {
    if (!this.isFormValid) {
      this.message.warn("Please complete the form properly.");
      return;
    }
    if (!this.validateStartEndTime()) {
      return;
    }
    if (!this.validateAppointmentDate()) {
      return;
    }
    const input = new CreateUpdateAppointmentDto();
    input.id = 0;
    input.tenantId = abp.session.tenantId;
    input.appointmentDate = this.appointment.appointmentDate;
    input.startTime = this.dateToTimeString(this.appointment.startTime);
    input.endTime = this.dateToTimeString(this.appointment.endTime);
    input.reasonForVisit = this.appointment.reasonForVisit;
    input.status = this.appointment.status;
    input.isFollowUp = this.appointment.isFollowUp;
    input.patientId = this.appointment.patientId;
    input.doctorId = this.appointment.doctorId;
    input.nurseId = this.appointment.nurseId;
    this._appoinmentService.createAppoinment(input).subscribe({
      next: (res) => {
        this.notify.info(this.l('SavedSuccessfully'));
        this.bsModalRef.hide();
        this.onSave.emit();
      },
      error: (err) => {
        this.saving = false;
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
