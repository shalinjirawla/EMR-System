import { Component, Injector, Input, OnInit, ViewChild } from '@angular/core';
import { FormsModule, NgForm } from '@node_modules/@angular/forms';
import { BsModalRef, BsModalService } from 'ngx-bootstrap/modal';
import { AppComponentBase } from '@shared/app-component-base';
import { DatePickerModule } from 'primeng/datepicker';
import { InputTextModule } from 'primeng/inputtext';
import { SelectModule } from 'primeng/select';
import { TextareaModule } from 'primeng/textarea';
import { PatientDto, NurseDto, CreateUpdateVitalDto, PatientServiceProxy, NurseServiceProxy, VitalServiceProxy, PatientDropDownDto } from '@shared/service-proxies/service-proxies';
import moment from 'moment';
import { AbpModalHeaderComponent } from "../../../shared/components/modal/abp-modal-header.component";
import { AbpModalFooterComponent } from '@shared/components/modal/abp-modal-footer.component';
import { CommonModule } from '@node_modules/@angular/common';
import { ChangeDetectorRef } from '@angular/core';


@Component({
  selector: 'app-edit-vitals',
  imports: [CommonModule, FormsModule, AbpModalHeaderComponent, AbpModalFooterComponent, SelectModule, DatePickerModule, InputTextModule, TextareaModule],
  providers: [PatientServiceProxy, NurseServiceProxy, VitalServiceProxy],
  templateUrl: './edit-vitals.component.html',
  styleUrl: './edit-vitals.component.css'
})
export class EditVitalsComponent extends AppComponentBase implements OnInit {
  @ViewChild('editVitalForm', { static: true }) editVitalForm: NgForm;

  @Input() vitalId: number; // <-- Receive vital ID for editing
  saving = false;

  patients: PatientDropDownDto[] = [];
  nurses: NurseDto[] = [];

  vital: CreateUpdateVitalDto = new CreateUpdateVitalDto();

  constructor(
    injector: Injector,
    public bsModalRef: BsModalRef,
    private _patientService: PatientServiceProxy,
    private _nurseService: NurseServiceProxy,
    private _vitalService: VitalServiceProxy,
    private cdr: ChangeDetectorRef
  ) {
    super(injector);
  }

  ngOnInit(): void {
    this.loadPatients();
    this.loadNurses();


    if (this.vitalId) {
      this._vitalService.get(this.vitalId).subscribe({
        next: (res) => {

          this.vital = new CreateUpdateVitalDto();
          this.vital.id = res.id;
          this.vital.tenantId = res.tenantId;
          this.vital.dateRecorded = moment(res.dateRecorded);
          this.vital.bloodPressure = res.bloodPressure;
          this.vital.heartRate = res.heartRate;
          this.vital.respirationRate = res.respirationRate;
          this.vital.temperature = res.temperature;
          this.vital.oxygenSaturation = res.oxygenSaturation;
          this.vital.height = res.height;
          this.vital.weight = res.weight;
          this.vital.bmi = res.bmi;
          this.vital.notes = res.notes;
          this.vital.patientId = res.patient.id;
          this.vital.nurseId = res.nurse.id;
          this.cdr.detectChanges();
        },
        error: (err) => console.error(err)
      });
    }

  }

  loadPatients() {
    this._patientService.patientDropDown().subscribe({
      next: res => {
        this.patients = res;

        // Now assign patientId if already fetched
        if (this.vitalId && this.vital.patientId) {
          this.vital.patientId = this.vital.patientId;
          this.cdr.detectChanges();
        }
      },
      error: err => console.error(err)
    });
  }

  loadNurses() {
    this._nurseService.getAllNursesByTenantID(abp.session.tenantId).subscribe({
      next: res => {
        this.nurses = res.items;

        if (this.vitalId && this.vital.nurseId) {
          this.vital.nurseId = this.vital.nurseId;
          this.cdr.detectChanges();
        }
      },
      error: err => console.error(err)
    });
  }


  save(): void {
    if (this.editVitalForm.invalid) {
      return;
    }

    this.saving = true;

    if (this.vitalId) {
      // Update flow
      this._vitalService.update(this.vital).subscribe({
        next: () => {
          this.notify.info(this.l('UpdatedSuccessfully'));
          this.bsModalRef.hide();
        },
        error: (err) => {
          console.error(err);
          this.saving = false;
        }
      });
    }
  }
}