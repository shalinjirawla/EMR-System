import { Component, Injector, OnInit, ViewChild, ChangeDetectorRef, EventEmitter, Output, AfterViewInit } from '@angular/core';
import { AbstractControl, FormsModule, NgForm, ValidationErrors } from '@angular/forms';
import { AppComponentBase } from '@shared/app-component-base';
import { BsModalRef, BsModalService } from 'ngx-bootstrap/modal';
import { CommonModule } from '@angular/common';
import { AbpModalHeaderComponent } from '../../../shared/components/modal/abp-modal-header.component';
import { AbpModalFooterComponent } from '../../../shared/components/modal/abp-modal-footer.component';
import { SelectModule } from 'primeng/select';
import { DatePickerModule } from 'primeng/datepicker';
import {
  CreateUpdateEmergencyCaseDto,
  DoctorDto,
  DoctorServiceProxy,
  EmergencyCaseDto,
  EmergencyServiceProxy,
  EmergencySeverity,
  EmergencyStatus,
  ModeOfArrival,
  NurseDto,
  NurseServiceProxy,
  PatientDropDownDto,
  PatientServiceProxy,
} from '@shared/service-proxies/service-proxies';
import moment from 'moment';
import { CreateAddmissionComponent } from '@app/admission/create-addmission/create-addmission.component';
import { CreateUserDialogComponent } from '@app/users/create-user/create-user-dialog.component';

@Component({
  selector: 'app-edit-emergency-case',
  imports: [
    FormsModule,
    CommonModule,
    SelectModule,
    DatePickerModule,
    AbpModalHeaderComponent,
    AbpModalFooterComponent,
  ],
  providers: [PatientServiceProxy, DoctorServiceProxy, NurseServiceProxy, EmergencyServiceProxy],
  templateUrl: './edit-emergency-case.component.html',
  styleUrl: './edit-emergency-case.component.css'
})
export class EditEmergencyCaseComponent extends AppComponentBase implements OnInit, AfterViewInit {
  @ViewChild('editEmergencyForm', { static: true }) editEmergencyForm: NgForm;
  @Output() onSave = new EventEmitter<void>();

  saving = false;
  patients: PatientDropDownDto[] = [];
  doctors: DoctorDto[] = [];
  nurses: NurseDto[] = [];
  uiArrivalTime: Date | null = null;

  id: number; // will be passed from parent (list component)
  emergency: EmergencyCaseDto = new EmergencyCaseDto();

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
    { label: 'PendingTriage', value: 0 },
    { label: 'Waiting', value: 1 },
    { label: 'InTreatment', value: 2 },
    { label: 'AdmissionPending', value: 3 },
    { label: 'Admitted', value: 4 },
    { label: 'Discharged', value: 5 }
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
    private _modalService: BsModalService,
  ) {
    super(injector);
  }

  ngOnInit(): void {
    this.loadPatients();
    this.loadDoctors();
    this.loadNurses();

    if (this.id) {
      this._emergencyService.get(this.id).subscribe(res => {
        this.emergency = res;

        // Convert Moment/string -> Date for the datepicker
        this.uiArrivalTime = this.toDate(res.arrivalTime);

        this.cd.detectChanges();
      });
    }
  }

  ngAfterViewInit() {
    // Add custom validator to patientId control
    const patientControl = this.editEmergencyForm.form.get('patientId');
    if (patientControl) {
      patientControl.setValidators(this.patientValidator.bind(this));
    }
  }

  private toDate(val: any): Date | null {
    if (!val) return null;
    // ABP proxies usually give moment.Moment; but handle ISO string too
    return moment.isMoment(val) ? val.toDate() : new Date(val);
  }

  loadPatients() {
    this._patientService.getOpdPatients().subscribe(res => {
      this.patients = res;
      this.cd.detectChanges();
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
    if (!this.editEmergencyForm?.form?.valid) {
      this.message.warn('Please complete the form properly.');
      return;
    }
    this.saving = true;

    const input = new CreateUpdateEmergencyCaseDto();
    input.id = this.emergency.id;
    input.emergencyNumber = this.emergency.emergencyNumber;
    input.tenantId = abp.session.tenantId;
    input.patientId = this.emergency.patientId;
    input.doctorId = this.emergency.doctorId;
    input.nurseId = this.emergency.nurseId;
    input.modeOfArrival = this.emergency.modeOfArrival;
    input.severity = this.emergency.severity;
    input.status = this.emergency.status;
    input.arrivalTime = this.uiArrivalTime ? moment(this.uiArrivalTime) : undefined;
    if (admissionId) {
      input.admissionsId = admissionId;
    }

    this._emergencyService.updateEmergencyCase(input).subscribe({
      next: () => {
        this.notify.info(this.l('UpdatedSuccessfully'));
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
    debugger
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
    const patientControl = this.editEmergencyForm.form.controls['patientId'];
    if (patientControl) {
      patientControl.updateValueAndValidity();
    }
  }
}
