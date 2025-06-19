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
import { AppointmentDto, AppointmentStatus, CreateUpdateAppointmentDto, DoctorDto, DoctorServiceProxy, NurseDto, NurseServiceProxy, PatientDto, PatientServiceProxy } from '@shared/service-proxies/service-proxies';
import { DatePickerModule } from 'primeng/datepicker';
import { CheckboxModule } from 'primeng/checkbox';
import { TextareaModule } from 'primeng/textarea';
import { CreateUserDialogComponent } from '@app/users/create-user/create-user-dialog.component';
import { PermissionCheckerService } from '@node_modules/abp-ng2-module';
import moment from 'moment';

@Component({
  selector: 'app-create-appoinment',
  imports: [
    FormsModule,
    AbpModalHeaderComponent,
    AbpModalFooterComponent,
    CommonModule, DatePickerModule, TextareaModule,
    SelectModule, ButtonModule, CheckboxModule
  ],
  providers: [DoctorServiceProxy, NurseServiceProxy, PatientServiceProxy, AppSessionService],
  standalone: true,
  templateUrl: './create-appoinment.component.html',
  styleUrl: './create-appoinment.component.css'
})
export class CreateAppoinmentComponent extends AppComponentBase implements OnInit {
  @ViewChild('createAppoinmentModal', { static: true }) createAppoinmentModal: NgForm;
  saving = false;
  patients!: PatientDto[];
  nurse!: NurseDto[];
  doctors!: DoctorDto[];
  statusOptions!: any[];
  showAddPatientButton = false;
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
    private _modalService: BsModalService,
    private permissionChecker: PermissionCheckerService
  ) {
    super(injector);
  }

  ngOnInit(): void {
    this.showAddPatientButton = this.permissionChecker.isGranted('Pages.Users');
    this.LoadDoctors();
    this.LoadNurse();
    this.LoadPatients();
    this.LoadStatus();
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
  showCreatePatientDialog(id?: number): void {
    let createOrEditPatientDialog: BsModalRef;
    if (!id) {
      createOrEditPatientDialog = this._modalService.show(CreateUserDialogComponent, {
        class: 'modal-lg',
        initialState: {
          defaultRole: 'Patient',
          disableRoleSelection: true
        }
      });
    }
  }
  save(): void {
    if (!this.appointment.patientId || !this.appointment.doctorId || !this.appointment.status || !this.appointment.appointmentDate) {
      console.warn("Form is invalid");
      return;
    }
    // this.saving = true;
    console.log('Saving appointment', this.appointment);
  }
}
