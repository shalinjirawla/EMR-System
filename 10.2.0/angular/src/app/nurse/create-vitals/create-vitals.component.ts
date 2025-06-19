import { Component, Injector, OnInit, ViewChild } from '@angular/core';
import { CommonModule } from '@node_modules/@angular/common';
import { FormsModule, NgForm } from '@node_modules/@angular/forms';
import { BsModalRef, BsModalService } from 'ngx-bootstrap/modal';
import { DatePickerModule } from 'primeng/datepicker';
import { InputTextModule } from 'primeng/inputtext';
import { SelectModule } from 'primeng/select';
import { TextareaModule } from 'primeng/textarea';
import { AbpModalFooterComponent } from '@shared/components/modal/abp-modal-footer.component';
import { AbpModalHeaderComponent } from '@shared/components/modal/abp-modal-header.component';
import { PatientDto, NurseDto, PatientServiceProxy, NurseServiceProxy, VitalServiceProxy, CreateUpdateVitalDto } from '@shared/service-proxies/service-proxies';
import { AppComponentBase } from '@shared/app-component-base';
import moment from 'moment';

@Component({
  selector: 'app-create-vitals',
  imports: [CommonModule, FormsModule, AbpModalHeaderComponent, AbpModalFooterComponent, SelectModule, DatePickerModule, InputTextModule, TextareaModule],
  providers: [PatientServiceProxy, NurseServiceProxy, VitalServiceProxy],
  templateUrl: './create-vitals.component.html',
  styleUrl: './create-vitals.component.css'
})
export class CreateVitalsComponent extends AppComponentBase implements OnInit {
  @ViewChild('createVitalForm', { static: true }) createVitalForm: NgForm;
  saving = false;

  patients: PatientDto[] = [];
  nurses: NurseDto[] = [];

  vital: CreateUpdateVitalDto = new CreateUpdateVitalDto();

  constructor(
    injector: Injector,
    public bsModalRef: BsModalRef,
    private _patientService: PatientServiceProxy,
    private _nurseService: NurseServiceProxy,
    private _vitalService: VitalServiceProxy
  ) {
    super(injector);
  }

  ngOnInit(): void {
    this.vital.tenantId = abp.session.tenantId!;
    this.vital.dateRecorded = moment();

    this.loadPatients();
    this.loadNurses();
  }

  loadPatients() {
    this._patientService.getAllPatientByTenantID(abp.session.tenantId).subscribe({
      next: res => this.patients = res.items,
      error: err => console.error(err)
    });
  }

  loadNurses() {
    this._nurseService.getAllNursesByTenantID(abp.session.tenantId).subscribe({
      next: res => this.nurses = res.items,
      error: err => console.error(err)
    });
  }

  save(): void {
    if (this.createVitalForm.invalid) {
      console.warn("Form is invalid");
      return;
    }

    const input = new CreateUpdateVitalDto();
    input.tenantId = this.vital.tenantId;
    input.dateRecorded = this.vital.dateRecorded;
    input.bloodPressure = this.vital.bloodPressure;
    input.heartRate = this.vital.heartRate;
    input.respirationRate = this.vital.respirationRate;
    input.temperature = this.vital.temperature;
    input.oxygenSaturation = this.vital.oxygenSaturation;
    input.height = this.vital.height;
    input.weight = this.vital.weight;
    input.bmi = this.vital.bmi;
    input.notes = this.vital.notes;
    input.patientId = this.vital.patientId;
    input.nurseId = this.vital.nurseId;

    this.saving = true;
    this._vitalService.create(input).subscribe({
      next: () => {
        this.notify.info(this.l('SavedSuccessfully'));
        this.bsModalRef.hide();
      },
      error: (err) => {
        console.error('Error saving vital', err);
        this.saving = false;
      }
    });
  }
}
