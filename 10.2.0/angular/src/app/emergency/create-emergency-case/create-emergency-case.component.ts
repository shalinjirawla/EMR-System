import { Component, Injector, OnInit, ViewChild, ChangeDetectorRef, EventEmitter, Output } from '@angular/core';
import { FormsModule, NgForm } from '@angular/forms';
import { AppComponentBase } from '@shared/app-component-base';
import { BsModalRef } from 'ngx-bootstrap/modal';
import { CommonModule } from '@angular/common';
import { AbpModalHeaderComponent } from '../../../shared/components/modal/abp-modal-header.component';
import { AbpModalFooterComponent } from '../../../shared/components/modal/abp-modal-footer.component';
import { SelectModule } from 'primeng/select';
import { DatePickerModule } from 'primeng/datepicker';
import { CreateUpdateEmergencyCaseDto, DoctorDto, DoctorServiceProxy, EmergencyServiceProxy, EmergencySeverity, EmergencyStatus, ModeOfArrival, NurseDto, NurseServiceProxy, PatientDropDownDto, PatientServiceProxy } from '@shared/service-proxies/service-proxies';

@Component({
  selector: 'app-create-emergency-case',
   imports: [
    FormsModule,
    CommonModule,
    SelectModule,
    DatePickerModule,
    AbpModalHeaderComponent,
    AbpModalFooterComponent,
  ],
  providers: [PatientServiceProxy, DoctorServiceProxy, NurseServiceProxy, EmergencyServiceProxy],
  templateUrl: './create-emergency-case.component.html',
  styleUrl: './create-emergency-case.component.css'
})
export class CreateEmergencyCaseComponent extends AppComponentBase implements OnInit {
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
    { label: 'Serious', value: EmergencySeverity._1},
    { label: 'Stable', value: EmergencySeverity._2 }
  ];

  statusOptions = [
    { label: 'Ongoing', value: EmergencyStatus._0 },
    { label: 'Discharged', value: EmergencyStatus._1 },
    { label: 'Admitted', value: EmergencyStatus._2},
    { label: 'Expired', value: EmergencyStatus._3 }
  ];

  constructor(
    injector: Injector,
    public bsModalRef: BsModalRef,
    private cd: ChangeDetectorRef,
    private _patientService: PatientServiceProxy,
    private _doctorService: DoctorServiceProxy,
    private _nurseService: NurseServiceProxy,
    private _emergencyService: EmergencyServiceProxy
  ) {
    super(injector);
  }

  ngOnInit(): void {
    this.loadPatients();
    this.loadDoctors();
    this.loadNurses();
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

  save() {
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
    input.arrivalTime = this.emergency.arrivalTime;

    this._emergencyService.create(input).subscribe({
      next: () => {
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
}