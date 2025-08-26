import { Component, Injector, OnInit, ViewChild, ChangeDetectorRef, EventEmitter, Output, AfterViewInit } from '@angular/core';
import { AbstractControl, FormsModule, NgForm, ValidationErrors } from '@angular/forms';
import { AppComponentBase } from '@shared/app-component-base';
import { BsModalRef, BsModalService } from 'ngx-bootstrap/modal';
import { CommonModule } from '@angular/common';
import { AbpModalHeaderComponent } from '../../../shared/components/modal/abp-modal-header.component';
import { AbpModalFooterComponent } from '../../../shared/components/modal/abp-modal-footer.component';
import { SelectModule } from 'primeng/select';
import { DatePickerModule } from 'primeng/datepicker';
import { CreateUpdateEmergencyCaseDto, DoctorDto, DoctorServiceProxy, EmergencyServiceProxy, EmergencySeverity, EmergencyStatus, ModeOfArrival, NurseDto, NurseServiceProxy, PatientDropDownDto, PatientServiceProxy } from '@shared/service-proxies/service-proxies';
import { ButtonModule } from 'primeng/button';
import { PermissionCheckerService } from '@node_modules/abp-ng2-module';
import { CreateUserDialogComponent } from '@app/users/create-user/create-user-dialog.component';
import { CreateAddmissionComponent } from '@app/admission/create-addmission/create-addmission.component';
import moment from '@node_modules/moment-timezone';
@Component({
  selector: 'app-create-emergency-case',
  imports: [
    FormsModule,
    CommonModule,
    SelectModule,
    DatePickerModule,
    AbpModalHeaderComponent,
    AbpModalFooterComponent, ButtonModule
  ],
  providers: [PatientServiceProxy, DoctorServiceProxy, NurseServiceProxy, EmergencyServiceProxy],
  templateUrl: './create-emergency-case.component.html',
  styleUrl: './create-emergency-case.component.css'
})
export class CreateEmergencyCaseComponent extends AppComponentBase implements OnInit, AfterViewInit {
  @ViewChild('createEmergencyForm', { static: true }) createEmergencyForm: NgForm;
  @Output() onSave = new EventEmitter<void>();

  saving = false;
  patients: PatientDropDownDto[] = [];
  doctors: DoctorDto[] = [];
  nurses: NurseDto[] = [];

  emergency: any = {
    tenantId: abp.session.tenantId,
    patientId: null,
    doctorId: null,
    nurseId: null,
    modeOfArrival: null,
    severity: null,
    status: EmergencyStatus._0,
    arrivalTime: new Date()
  };

  modeOfArrivalOptions = [
    { label: 'Walk In', value: ModeOfArrival._0 },
    { label: 'Ambulance', value: ModeOfArrival._1 },
    { label: 'Police', value: ModeOfArrival._2 },
    { label: 'Unknown', value: ModeOfArrival._3 }
  ];

  severityOptions = [
    { label: 'Critical', value: EmergencySeverity._0 },
    { label: 'Serious', value: EmergencySeverity._1 },
    { label: 'Stable', value: EmergencySeverity._2 }
  ];

  statusOptions = [
    { label: 'PendingTriage', value: EmergencyStatus._0 },
    { label: 'Waiting', value: EmergencyStatus._1 },
    { label: 'InTreatment', value: EmergencyStatus._2 },
    { label: 'AdmissionPending', value: EmergencyStatus._3 },
    { label: 'Admitted', value: EmergencyStatus._4 },
    // { label: 'Discharged', value: EmergencyStatus._5 }
  ];
  showAddPatientButton = false;
  isPatientRequired = false;
  constructor(
    injector: Injector,
    public bsModalRef: BsModalRef,
    private cd: ChangeDetectorRef,
    private _patientService: PatientServiceProxy,
    private _doctorService: DoctorServiceProxy,
    private _nurseService: NurseServiceProxy,
    private _emergencyService: EmergencyServiceProxy,
    private permissionChecker: PermissionCheckerService,
    private _modalService: BsModalService,
  ) {
    super(injector);
  }

  ngOnInit(): void {
    this.showAddPatientButton = this.permissionChecker.isGranted('Pages.Users');
    this.loadPatients();
    this.loadDoctors();
    this.loadNurses();
  }

  ngAfterViewInit() {
    // Add custom validator to patientId control
    const patientControl = this.createEmergencyForm.form.get('patientId');
    if (patientControl) {
      patientControl.setValidators(this.patientValidator.bind(this));
    }
  }

  loadPatients() {
    this._patientService.getOpdPatients().subscribe(res => {
      this.patients = res;
    });
  }

  loadDoctors() {
    this._doctorService.getAllDoctorsByTenantID(abp.session.tenantId).subscribe(res => {
      this.doctors = res.items;
      this.cd.detectChanges();
    });
  }

  loadNurses() {
    this._nurseService.getAllNursesByTenantID(abp.session.tenantId).subscribe(res => {
      this.nurses = res.items;
      this.cd.detectChanges();
    });
  }

  FormSubmit(admissionId: any) {
    if (!this.createEmergencyForm?.form?.valid) {
      this.message.warn('Please complete the form properly.');
      return;
    }
    this.saving = true;

    const input = new CreateUpdateEmergencyCaseDto();
    input.tenantId = abp.session.tenantId;
    input.patientId = this.emergency.patientId;
    input.doctorId = this.emergency.doctorId;
    input.nurseId = this.emergency.nurseId;
    input.modeOfArrival = this.emergency.modeOfArrival;
    input.severity = this.emergency.severity;
    input.status = this.emergency.status;
    input.arrivalTime = null;
    if (admissionId) {
      input.admissionsId = admissionId;
    }
    this._emergencyService.create(input).subscribe({
      next: (res) => {
        this.notify.info(this.l('SavedSuccessfully'));
        this.saving = false;
        this.bsModalRef.hide();
        this.onSave.emit();
      },
      error: () => {
        this.saving = false;
      }
    });
  }

  save() {
    if (this.emergency.status === EmergencyStatus._4) {
      this.FillAdmissionForm(this.emergency);
    }
    else {
      this.FormSubmit(null);
    }
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
      this.loadPatients();
    });
    // Also subscribe to the onHide event to handle cases where modal closes without saving
    createOrEditPatientDialog.onHide.subscribe(() => {
      this.loadPatients(); // Refresh when modal closes regardless of save action
    });
  }
  statusChange(event: any) {
    const isWalkIn = this.emergency.modeOfArrival === ModeOfArrival._0;
    const isAdmitted = event?.value === EmergencyStatus._4;

    this.isPatientRequired = isWalkIn || isAdmitted;
    this.updatePatientValidation();
  }
  patientValidator(control: AbstractControl): ValidationErrors | null {
    if (this.isPatientRequired && !control.value) {
      return { required: true };
    }
    return null;
  }

  FillAdmissionForm(emengencyData: any) {
    let fillAdmissionForm: BsModalRef;
    fillAdmissionForm = this._modalService.show(CreateAddmissionComponent, {
      class: 'modal-lg',
      initialState: {
        selectedPatientId: emengencyData.patientId,
        selectedNurseId: emengencyData.nurseId,
        selectDoctorId: emengencyData.doctorId,
        admissionType: emengencyData.status === EmergencyStatus._4 ? emengencyData.status : 0,
        disableRoleSelection: true
      }
    });

    fillAdmissionForm.content.onSave.subscribe((res) => {
      this.FormSubmit(res);
    });
  }

  onChangeModeOfArrival(event: any) {
    const isWalkIn = event?.value === ModeOfArrival._0;
    const isAdmitted = this.emergency.status === EmergencyStatus._4;
    this.isPatientRequired = isWalkIn || isAdmitted;
    this.updatePatientValidation();
  }

  updatePatientValidation() {
    const patientControl = this.createEmergencyForm.form.controls['patientId'];
    if (patientControl) {
      patientControl.updateValueAndValidity();
    }
  }
}