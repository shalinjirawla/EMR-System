import { Component, Injector, OnInit, ViewChild, ChangeDetectorRef, EventEmitter, Output } from '@angular/core';
import { FormsModule, NgForm } from '@angular/forms';
import { AppComponentBase } from '@shared/app-component-base';
import { BsModalRef } from 'ngx-bootstrap/modal';
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
export class EditEmergencyCaseComponent extends AppComponentBase implements OnInit {
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
    { label: 'Ongoing', value: EmergencyStatus._0 },
    { label: 'Discharged', value: EmergencyStatus._1 },
    { label: 'Admitted', value: EmergencyStatus._2 },
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

    if (this.id) {
      this._emergencyService.get(this.id).subscribe(res => {
        this.emergency = res;

        // Convert Moment/string -> Date for the datepicker
        this.uiArrivalTime = this.toDate(res.arrivalTime);

        this.cd.detectChanges();
      });
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

  save() {
    if (!this.editEmergencyForm?.form?.valid) {
      this.message.warn('Please complete the form properly.');
      return;
    }
    this.saving = true;

    const input = new CreateUpdateEmergencyCaseDto();
    input.id = this.emergency.id;
    input.tenantId = abp.session.tenantId;
    input.patientId = this.emergency.patientId;
    input.doctorId = this.emergency.doctorId;
    input.nurseId = this.emergency.nurseId;
    input.modeOfArrival = this.emergency.modeOfArrival;
    input.severity = this.emergency.severity;
    input.status = this.emergency.status;
    input.arrivalTime = this.uiArrivalTime ? moment(this.uiArrivalTime) : undefined;
    debugger

    this._emergencyService.update(input).subscribe({
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
}
